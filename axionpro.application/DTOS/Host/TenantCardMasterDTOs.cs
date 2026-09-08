// ================================================================
// Purpose : Defines Host-only Tenant card inventory contracts. Tenant users
//           can bind an active card, but never create or price inventory.
// ================================================================

using axionpro.domain.Entity;

namespace axionpro.application.DTOS.Host;

/// <summary>Host-editable procurement and lifecycle data for one physical Tenant card.</summary>
public class TenantCardMasterRequestDTO : TenantDeviceAccessRequestDTO
{
    /// <summary>Card reader number. It is encrypted at rest and never returned in full.</summary>
    public string CardNumber { get; set; } = string.Empty;
    public string? CardReference { get; set; }
    public string PurchaseCurrencyCode { get; set; } = "INR";
    public decimal UnitPurchasePriceExcludingTax { get; set; }
    public int? SupplierCountryId { get; set; }
    public int? SupplierStateId { get; set; }
    public int? PlaceOfSupplyCountryId { get; set; }
    public int? PlaceOfSupplyStateId { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierTaxRegistrationNumber { get; set; }
    public string? PurchaseInvoiceNumber { get; set; }
    public DateOnly? PurchaseInvoiceDate { get; set; }
    public TenantCardTaxTreatment TaxTreatment { get; set; }
    public decimal CgstRate { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstRate { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstRate { get; set; }
    public decimal IgstAmount { get; set; }
    public string? ForeignTaxLabel { get; set; }
    public decimal ForeignTaxRate { get; set; }
    public decimal ForeignTaxAmount { get; set; }
    public decimal CustomsDutyAmount { get; set; }
    public decimal FreightAmount { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Creates an available physical card in a Host-selected Tenant inventory.</summary>
public sealed class CreateTenantCardMasterRequestDTO : TenantCardMasterRequestDTO { }

/// <summary>Updates an unassigned or otherwise eligible physical card.</summary>
public sealed class UpdateTenantCardMasterRequestDTO : TenantCardMasterRequestDTO
{
    public string Id { get; set; } = string.Empty;
}

/// <summary>Changes a Host-managed card's active state without exposing the card number.</summary>
public sealed class UpdateTenantCardMasterStatusRequestDTO : TenantDeviceAccessRequestDTO
{
    public string Id { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>Filters a Host-selected Tenant card inventory.</summary>
public sealed class TenantCardMasterFilterRequestDTO : TenantDeviceAccessRequestDTO
{
    public string? Search { get; set; }
    public TenantCardStatus? CardStatus { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>Returns non-sensitive Host card inventory information.</summary>
public sealed class TenantCardMasterResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string MaskedCardNumber { get; set; } = string.Empty;
    public string? CardReference { get; set; }
    public TenantCardStatus CardStatus { get; set; }
    public string PurchaseCurrencyCode { get; set; } = string.Empty;
    public decimal UnitPurchasePriceExcludingTax { get; set; }
    public int? SupplierCountryId { get; set; }
    public int? SupplierStateId { get; set; }
    public int? PlaceOfSupplyCountryId { get; set; }
    public int? PlaceOfSupplyStateId { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierTaxRegistrationNumber { get; set; }
    public string? PurchaseInvoiceNumber { get; set; }
    public DateOnly? PurchaseInvoiceDate { get; set; }
    public TenantCardTaxTreatment TaxTreatment { get; set; }
    public decimal CgstRate { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstRate { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstRate { get; set; }
    public decimal IgstAmount { get; set; }
    public string? ForeignTaxLabel { get; set; }
    public decimal ForeignTaxRate { get; set; }
    public decimal ForeignTaxAmount { get; set; }
    public decimal CustomsDutyAmount { get; set; }
    public decimal FreightAmount { get; set; }
    public decimal LandedCost { get; set; }
    public bool IsActive { get; set; }
    public DateTime AddedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
