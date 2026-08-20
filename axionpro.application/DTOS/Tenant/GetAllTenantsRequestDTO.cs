// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines Host-side filters for retrieving Tenant management records.
// ================================================================

namespace axionpro.application.DTOs.Tenant;

/// <summary>
/// Represents optional filters and paging values for the Tenant management list.
/// </summary>
public sealed class GetAllTenantsRequestDTO
{
    #region Filter Properties

    /// <summary>
    /// Gets or sets the optional Tenant active-state filter.
    /// A <see langword="null"/> value represents all active states.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the optional Tenant verification-state filter.
    /// A <see langword="null"/> value represents all verification states.
    /// </summary>
    public bool? IsVerified { get; set; }

    /// <summary>
    /// Gets or sets the optional text used to search Tenant management records.
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// Gets or sets the requested one-based page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the requested page size.
    /// </summary>
    public int PageSize { get; set; } = 10;

    #endregion
}
