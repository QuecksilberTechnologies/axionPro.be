using axionpro.application.Constants;
using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

/// <summary>
/// Verifies that Host entitlement synchronization grants only missing enabled operations
/// to the Tenant Admin role without creating duplicate permission rows.
/// </summary>
[TestFixture]
[Category("TenantEntitlementPermissionSyncDatabase")]
[NonParallelizable]
public sealed class TenantAdminEntitlementPermissionSyncDatabaseTests
{
    [Test]
    public void Entitlement_command_persists_snapshot_before_missing_admin_permission_sync()
    {
        var repositoryRoot = FindRepositoryRoot();
        var commandSource = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "axionpro.application",
            "Features",
            "TenantManagementCmd",
            "Commands",
            "SynchronizeTenantPlanEntitlementsCommand.cs"));
        var repositorySource = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "axionpro.persistance",
            "Repositories",
            "UserRolesPermissionOnModuleRepository.cs"));

        var firstSave = commandSource.IndexOf(
            "SaveChangesAsync(cancellationToken)",
            StringComparison.Ordinal);
        var permissionSync = commandSource.IndexOf(
            "AddMissingTenantAdminPermissionsAsync",
            StringComparison.Ordinal);

        Assert.Multiple(() =>
        {
            Assert.That(firstSave, Is.GreaterThanOrEqualTo(0));
            Assert.That(permissionSync, Is.GreaterThan(firstSave));
            Assert.That(repositorySource, Does.Contain("operation.TenantId == tenantId && operation.IsEnabled"));
            Assert.That(repositorySource, Does.Contain("permission.RoleId == adminRole.Id"));
            Assert.That(repositorySource, Does.Contain("!existingPermissionKeys.Contains"));
            Assert.That(repositorySource, Does.Contain("RoleType == ConstantValues.RoleTypeAdmin"));
        });
    }

    [Test]
    public async Task Missing_admin_permissions_are_added_once_and_existing_rows_are_unchanged()
    {
        var connection = Environment.GetEnvironmentVariable("AXIONPRO_BULK_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connection))
        {
            Assert.Ignore("Requires disposable axionpro_bulk_test database.");
        }

        var connectionBuilder = new NpgsqlConnectionStringBuilder(connection);
        Assert.Multiple(() =>
        {
            Assert.That(connectionBuilder.Database, Is.EqualTo("axionpro_bulk_test"));
            Assert.That(connectionBuilder.Host, Is.AnyOf("127.0.0.1", "localhost"));
        });

        var options = new DbContextOptionsBuilder<WorkforceDbContext>()
            .UseNpgsql(connection)
            .Options;

        await using var context = new WorkforceDbContext(options);
        await using var transaction = await context.Database.BeginTransactionAsync();

        var adminRole = await context.Roles
            .AsNoTracking()
            .Where(role =>
                role.TenantId.HasValue &&
                role.RoleType == ConstantValues.RoleTypeAdmin &&
                role.IsActive &&
                role.IsSoftDeleted == false &&
                role.IsSystemDefault == false)
            .Where(role => context.TenantEnabledOperations.Any(operation =>
                operation.TenantId == role.TenantId && operation.IsEnabled))
            .FirstOrDefaultAsync();

        Assert.That(adminRole, Is.Not.Null, "The disposable database needs an active Tenant Admin with an enabled operation.");
        var tenantId = adminRole!.TenantId!.Value;

        var enabledOperation = await context.TenantEnabledOperations
            .AsNoTracking()
            .Where(operation => operation.TenantId == tenantId && operation.IsEnabled)
            .Select(operation => new { operation.ModuleId, operation.OperationId })
            .FirstAsync();

        await context.RoleModuleAndPermissions
            .Where(permission =>
                permission.RoleId == adminRole.Id &&
                permission.ModuleId == enabledOperation.ModuleId &&
                permission.OperationId == enabledOperation.OperationId)
            .ExecuteDeleteAsync();

        var repository = new UserRolesPermissionOnModuleRepository(
            context,
            NullLogger<UserRolesPermissionOnModuleRepository>.Instance);

        var firstAddedCount = await repository.AddMissingTenantAdminPermissionsAsync(
            tenantId,
            adminRole.AddedById);
        await context.SaveChangesAsync();

        var matchingCountAfterFirstSync = await context.RoleModuleAndPermissions.CountAsync(permission =>
            permission.RoleId == adminRole.Id &&
            permission.ModuleId == enabledOperation.ModuleId &&
            permission.OperationId == enabledOperation.OperationId);

        var secondAddedCount = await repository.AddMissingTenantAdminPermissionsAsync(
            tenantId,
            adminRole.AddedById);
        await context.SaveChangesAsync();

        var matchingCountAfterSecondSync = await context.RoleModuleAndPermissions.CountAsync(permission =>
            permission.RoleId == adminRole.Id &&
            permission.ModuleId == enabledOperation.ModuleId &&
            permission.OperationId == enabledOperation.OperationId);

        Assert.Multiple(() =>
        {
            Assert.That(firstAddedCount, Is.GreaterThanOrEqualTo(1));
            Assert.That(matchingCountAfterFirstSync, Is.EqualTo(1));
            Assert.That(secondAddedCount, Is.EqualTo(0));
            Assert.That(matchingCountAfterSecondSync, Is.EqualTo(1));
        });

        await transaction.RollbackAsync();
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root containing AxionPro.sln was not found.");
    }
}
