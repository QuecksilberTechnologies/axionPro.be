// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for deactivating a Tenant.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host-side request to deactivate a Tenant.
/// </summary>
/// <remarks>
/// The future handler must validate the Host with <c>ValidateHostUserRequestAsync()</c>,
/// set <c>Tenant.IsActive</c> and all corresponding <c>LoginCredential.IsActive</c> values to
/// <see langword="false"/>, and persist the change atomically. The request intentionally supplies
/// no client-controlled status or actor identifier.
/// </remarks>
public sealed class DeactivateTenantCommand : IRequest<ApiResponse<TenantResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateTenantCommand"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant deactivation request.</param>
    public DeactivateTenantCommand(DeactivateTenantRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant deactivation request, including its administrative remark.
    /// </summary>
    public DeactivateTenantRequestDTO RequestDTO { get; }
}

#endregion
