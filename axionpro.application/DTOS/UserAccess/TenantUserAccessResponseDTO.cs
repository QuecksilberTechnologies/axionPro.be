// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the lightweight Tenant employee runtime authorization bootstrap response.
// ================================================================

using axionpro.application.DTOs.RoleModulePermission;

namespace axionpro.application.DTOS.UserAccess;

/// <summary>
/// Represents the current Tenant-scoped operational navigation available to an authenticated employee.
/// </summary>
public sealed class TenantUserAccessResponseDTO
{
    #region Properties

    /// <summary>
    /// Gets or sets the current effective operational menu hierarchy.
    /// </summary>
    public IReadOnlyCollection<MainModuleDto> OperationalMenus { get; init; }
        = Array.Empty<MainModuleDto>();

    #endregion
}
