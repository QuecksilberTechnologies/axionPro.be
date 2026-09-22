using System.Reflection;
using System.Text;
using axionpro.application.Common.Models.Security;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.Exceptions;
using axionpro.application.Features.HolidayCalandarCmd;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("HolidayCalendarTransferDatabase")]
public sealed class HolidayCalendarTransferDatabaseTests
{
    [Test]
    public async Task Csv_import_export_skip_and_invalid_row_are_transactionally_safe()
    {
        var connectionString = Environment.GetEnvironmentVariable("AXIONPRO_HOLIDAY_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Ignore("Set AXIONPRO_HOLIDAY_TEST_CONNECTION for the rollback-only import/export test.");
        }

        var options = new DbContextOptionsBuilder<WorkforceDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var context = new WorkforceDbContext(options);
        await using var transaction = await context.Database.BeginTransactionAsync();
        var location = await context.TenantLocations.AsNoTracking()
            .Where(item => item.IsActive && !item.IsSoftDeleted)
            .OrderBy(item => item.Id)
            .FirstAsync();
        var repository = new HolidayCalandarRepository(
            context,
            NullLogger<HolidayCalandarRepository>.Instance);
        var actor = new CommonDecodedResult
        {
            Success = true,
            TenantId = location.TenantId,
            LoggedInEmployeeId = 4,
            RoleId = 2
        };
        var common = Proxy<ICommonRequestService>((_, _) => Task.FromResult(actor));
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name == "get_HolidayCalandarRepository"
            ? repository
            : throw new AssertionException($"Unexpected unit request: {method.Name}"));
        var importer = new ImportHolidaysCommandHandler(unit, common);
        var exporter = new ExportHolidaysQueryHandler(unit, common);
        var csv = "TenantLocationId,HolidayName,HolidayDate,IsOptional,Description\r\n"
            + $"{location.Id},Calendar Import Verification,2099-12-29,false,Annual office holiday\r\n";

        try
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var file = new FormFile(stream, 0, stream.Length, "File", "holidays.csv");
            var created = await importer.Handle(new ImportHolidaysCommand(new ImportHolidayRequestDTO
            {
                File = file
            }), CancellationToken.None);
            Assert.Multiple(() =>
            {
                Assert.That(created.Data.TotalRows, Is.EqualTo(1));
                Assert.That(created.Data.CreatedCount, Is.EqualTo(1));
                Assert.That(created.Data.SkippedExistingCount, Is.Zero);
            });

            var exported = await exporter.Handle(new ExportHolidaysQuery(new BasicRequestDTO
            {
                TenantLocationId = location.Id,
                HolidayYear = 2099
            }), CancellationToken.None);
            var exportedText = Encoding.UTF8.GetString(exported);
            Assert.That(exportedText, Does.Contain("Calendar Import Verification"));
            Assert.That(exportedText, Does.Contain("2099-12-29"));

            var repeated = await importer.Handle(new ImportHolidaysCommand(new ImportHolidayRequestDTO
            {
                PastedText = csv
            }), CancellationToken.None);
            Assert.That(repeated.Data.CreatedCount, Is.Zero);
            Assert.That(repeated.Data.SkippedExistingCount, Is.EqualTo(1));

            var invalid = csv.Replace("2099-12-29", "not-a-date");
            Assert.ThrowsAsync<ValidationErrorException>(async () =>
                await importer.Handle(new ImportHolidaysCommand(new ImportHolidayRequestDTO
                {
                    PastedText = invalid
                }), CancellationToken.None));
            Assert.That(await context.OrganizationHolidayCalendars.CountAsync(item =>
                item.TenantId == location.TenantId && item.HolidayName == "Calendar Import Verification"),
                Is.EqualTo(1));
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, BulkImportPermissionTests.TestProxy>();
        ((BulkImportPermissionTests.TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }
}
