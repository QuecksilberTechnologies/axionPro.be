using System.Reflection;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.Hash;
using axionpro.application.DTOS.Token.ems.application.DTOs.UserLogin;
using axionpro.application.Exceptions;
using axionpro.application.Features.UserLoginAndDashboardCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.domain.Entity;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture, Category("RefreshV2")]
public class RefreshTokenV2Tests
{
    #region Invalid Sessions

    [TestCase("missing")]
    [TestCase("expired")]
    [TestCase("revoked")]
    [TestCase("two-owners")]
    [TestCase("no-owner")]
    [TestCase("unknown-type")]
    public void Invalid_session_never_generates_or_rotates_tokens(string condition)
    {
        var token = HostToken();
        switch (condition)
        {
            case "expired": token.ExpiryDate = DateTime.UtcNow.AddMinutes(-1); break;
            case "revoked": token.IsRevoked = true; break;
            case "two-owners": token.LoginCredentialId = 2; break;
            case "no-owner": token.HostUserId = null; break;
            case "unknown-type": token.UserType = 99; break;
        }
        var repository = Proxy<IRefreshTokenRepository>((method, args) =>
        {
            Assert.That(method.Name, Is.EqualTo("GetByHashedTokenAsync"));
            Assert.That(args![0], Is.EqualTo(HashHelper.Sha256("fixture")));
            return Task.FromResult(condition == "missing" ? null : token);
        });
        var handler = Handler(Unexpected<IUnitOfWork>(), repository, Unexpected<ITokenService>());
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(
            new RefreshTokenV2Command(new RefreshTokenRequestDTO { RefreshToken = "fixture" }), default));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Empty_token_is_validation_error_without_database_work(string raw)
    {
        var handler = Handler(Unexpected<IUnitOfWork>(), Unexpected<IRefreshTokenRepository>(), Unexpected<ITokenService>());
        Assert.ThrowsAsync<ValidationErrorException>(async () => await handler.Handle(
            new RefreshTokenV2Command(new RefreshTokenRequestDTO { RefreshToken = raw }), default));
    }

    #endregion

    #region Host Eligibility And Rotation

    [TestCase("inactive-user")]
    [TestCase("deleted-user")]
    [TestCase("renamed-user")]
    [TestCase("inactive-role")]
    public void Host_eligibility_is_revalidated_before_rotation(string condition)
    {
        var host = new HostUser { Id = 1, HostRoleId = 1, LoginId = "fixture", IsActive = true };
        if (condition == "inactive-user") host.IsActive = false;
        if (condition == "deleted-user") host.IsSoftDeleted = true;
        if (condition == "renamed-user") host.LoginId = "changed";
        var users = Proxy<IHostUserRepository>((_, _) => Task.FromResult<HostUser?>(host));
        var roles = Proxy<IHostRoleRepository>((_, _) => Task.FromResult<HostRole?>(new HostRole { IsActive = false }));
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name switch
        {
            "get_HostUserRepository" => users,
            "get_HostRoleRepository" => roles,
            _ => throw new AssertionException("Unexpected dependency: " + method.Name)
        });
        var repository = Proxy<IRefreshTokenRepository>((method, _) => method.Name == "GetByHashedTokenAsync"
            ? Task.FromResult<RefreshToken?>(HostToken())
            : throw new AssertionException("Rotation must not run."));
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await Handler(unit, repository, Unexpected<ITokenService>()).Handle(
            new RefreshTokenV2Command(new RefreshTokenRequestDTO { RefreshToken = "fixture" }), default));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Host_rotation_avoids_permission_reads_and_rolls_back_failed_insert(bool insertionSucceeds)
    {
        var calls = new List<string>();
        var users = Proxy<IHostUserRepository>((_, _) => Task.FromResult<HostUser?>(
            new HostUser { Id = 1, HostRoleId = 1, LoginId = "fixture", IsActive = true }));
        var roles = Proxy<IHostRoleRepository>((_, _) => Task.FromResult<HostRole?>(new HostRole { IsActive = true }));
        var unit = Proxy<IUnitOfWork>((method, _) =>
        {
            calls.Add(method.Name);
            return method.Name switch
            {
                "get_HostUserRepository" => users,
                "get_HostRoleRepository" => roles,
                "BeginTransactionAsync" or "CommitTransactionAsync" or "RollbackTransactionAsync" => Task.CompletedTask,
                _ => throw new AssertionException("Unexpected presentation/permission dependency: " + method.Name)
            };
        });
        RefreshToken? inserted = null;
        var repository = Proxy<IRefreshTokenRepository>((method, args) =>
        {
            calls.Add(method.Name);
            switch (method.Name)
            {
                case "GetByHashedTokenAsync": return Task.FromResult<RefreshToken?>(HostToken());
                case "UpdateReplacedByTokenAsync":
                case "RevokeAsync": return Task.CompletedTask;
                case "InsertAsync":
                    inserted = (RefreshToken)args![0]!;
                    return Task.FromResult(insertionSucceeds);
                default: throw new AssertionException(method.Name);
            }
        });
        var expiry = DateTime.UtcNow.AddMinutes(15);
        var service = Proxy<ITokenService>((method, _) => method.Name switch
        {
            "GenerateHostToken" => Task.FromResult("access-fixture"),
            "GetExpiryFromToken" => (DateTime?)expiry,
            "GenerateRefreshToken" => Task.FromResult("replacement-fixture"),
            _ => throw new AssertionException(method.Name)
        });
        var handler = Handler(unit, repository, service);
        var command = new RefreshTokenV2Command(new RefreshTokenRequestDTO { RefreshToken = "fixture" });
        if (insertionSucceeds)
        {
            var result = await handler.Handle(command, default);
            Assert.That(result.Data!.TokenExpiry, Is.EqualTo(expiry));
            Assert.That(result.Data.RefreshTokenExpiresAtUtc, Is.EqualTo(inserted!.ExpiryDate));
            Assert.That(inserted.Token, Is.EqualTo(HashHelper.Sha256(result.Data.RefreshToken)));
            Assert.That(calls, Does.Contain("CommitTransactionAsync").And.Not.Contain("RollbackTransactionAsync"));
        }
        else
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Handle(command, default));
            Assert.That(calls, Does.Contain("RollbackTransactionAsync").And.Not.Contain("CommitTransactionAsync"));
        }
    }

    #endregion

    #region Test Helpers

    private static RefreshToken HostToken() => new()
    {
        Id = 1,
        HostUserId = 1,
        UserType = (short)LoginUserType.Host,
        LoginId = "fixture",
        ExpiryDate = DateTime.UtcNow.AddHours(1)
    };

    private static RefreshTokenV2CommandHandler Handler(IUnitOfWork unit, IRefreshTokenRepository repository, ITokenService tokens)
    {
        return new RefreshTokenV2CommandHandler(unit, tokens, repository, Unexpected<IIdEncoderService>(),
            NullLogger<RefreshTokenV2CommandHandler>.Instance);
    }

    private static T Unexpected<T>() where T : class => Proxy<T>((method, _) =>
        throw new AssertionException("Unexpected call: " + method.Name));

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, BulkImportPermissionTests.TestProxy>();
        ((BulkImportPermissionTests.TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }

    #endregion
}
