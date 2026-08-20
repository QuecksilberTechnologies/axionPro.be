// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for Tenant soft deletion.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host-side request to place a Tenant into the existing soft-delete lifecycle.
/// </summary>
/// <remarks>
/// The future handler must validate the Host with <c>ValidateHostUserRequestAsync()</c>,
/// apply the existing Tenant soft-delete convention, and perform dependency validation before persistence.
/// </remarks>
public sealed class DeleteTenantCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTenantCommand"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant deletion request.</param>
    public DeleteTenantCommand(DeleteTenantRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant deletion request.
    /// </summary>
    public DeleteTenantRequestDTO RequestDTO { get; }
}

#endregion
