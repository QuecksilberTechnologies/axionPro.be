using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("HostBulkImport")]
public sealed class HostBulkImportTableMapperTests
{
    [TestCase("TenantId")]
    [TestCase("IsOccupied")]
    [TestCase("TenantLocationId")]
    public void Assignment_columns_are_rejected_even_when_not_explicitly_mapped(string field)
    {
        Assert.Throws<ValidationErrorException>(() => HostBulkImportTableMapper.ResolveColumns(
            new[] { "DeviceCode", field }, HostBulkImportTableMapper.DeviceColumns, null));
    }

    [TestCase("TenantId")]
    [TestCase("ModuleId")]
    [TestCase("OperationId")]
    [TestCase("AddedById")]
    public void Card_spreadsheet_cannot_override_trusted_context(string field)
    {
        Assert.Throws<ValidationErrorException>(() => HostBulkImportTableMapper.ResolveColumns(
            new[] { "Input" }, HostBulkImportTableMapper.CardColumns,
            System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, string> { [field] = "Input" })));
    }

    [Test]
    public void Card_numbers_keep_leading_zeros_and_invoice_values_use_explicit_formats()
    {
        var table = new BulkImportTableDTO
        {
            Columns = ["CardNumber", "PurchaseInvoiceDate", "UnitPurchasePriceExcludingTax"],
            Rows = [new() { RowNumber = 2, Values = ["0000123456789012345", "2026-09-12", "12.50"] }]
        };
        var mapping = HostBulkImportTableMapper.ResolveColumns(table.Columns, HostBulkImportTableMapper.CardColumns, null);
        var dto = HostBulkImportTableMapper.ReadRow<CreateTenantCardMasterRequestDTO>(table, table.Rows[0], mapping);
        Assert.Multiple(() =>
        {
            Assert.That(dto.CardNumber, Is.EqualTo("0000123456789012345"));
            Assert.That(dto.PurchaseInvoiceDate, Is.EqualTo(new DateOnly(2026, 9, 12)));
            Assert.That(dto.UnitPurchasePriceExcludingTax, Is.EqualTo(12.50m));
            Assert.That(dto.PurchaseCurrencyCode, Is.EqualTo("INR"));
            Assert.That(dto.TenantId, Is.Null);
        });
    }

    [TestCase("PurchaseInvoiceDate", "12/09/2026")]
    [TestCase("UnitPurchasePriceExcludingTax", "1,200")]
    [TestCase("IsActive", "maybe")]
    [TestCase("TaxTreatment", "99999")]
    public void Invalid_formats_are_rejected_without_echoing_cell_data(string field, string value)
    {
        var table = new BulkImportTableDTO { Columns = [field] };
        var mapping = HostBulkImportTableMapper.ResolveColumns(table.Columns, HostBulkImportTableMapper.CardColumns, null);
        var error = Assert.Throws<ValidationErrorException>(() => HostBulkImportTableMapper.ReadRow<CreateTenantCardMasterRequestDTO>(
            table, new BulkImportSourceRowDTO { RowNumber = 2, Values = [value] }, mapping));
        Assert.That(error!.Message, Does.Not.Contain(value));
    }

    [Test]
    public void Nonstandard_headers_require_explicit_mapping()
    {
        Assert.That(HostBulkImportTableMapper.ResolveColumns(new[] { "Serial" }, HostBulkImportTableMapper.DeviceColumns, null), Is.Empty);
        var mapping = HostBulkImportTableMapper.ResolveColumns(new[] { "Serial" }, HostBulkImportTableMapper.DeviceColumns, "{\"SNo\":\"Serial\"}");
        var dto = HostBulkImportTableMapper.ReadRow<CreateDeviceMasterRequestDTO>(
            new BulkImportTableDTO { Columns = ["Serial"] }, new BulkImportSourceRowDTO { RowNumber = 2, Values = ["000123"] }, mapping);
        Assert.That(dto.SNo, Is.EqualTo("000123"));
    }

    [Test]
    public void Duplicate_headers_are_rejected()
    {
        Assert.Throws<ValidationErrorException>(() => HostBulkImportTableMapper.ResolveColumns(
            new[] { "CardNumber", "cardnumber" }, HostBulkImportTableMapper.CardColumns, null));
    }
}
