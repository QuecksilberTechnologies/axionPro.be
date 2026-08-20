// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for updating editable Tenant fields.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host-side request to update editable Tenant fields.
/// </summary>
/// <remarks>
/// The future handler must validate the Host with <c>ValidateHostUserRequestAsync()</c> and derive
/// all actor and audit information server-side. No Host user identifier or audit value is accepted from the client.
/// </remarks>
public sealed class UpdateTenantCommand : IRequest<ApiResponse<TenantResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTenantCommand"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant update request.</param>
    public UpdateTenantCommand(UpdateTenantRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant update request.
    /// </summary>
    public UpdateTenantRequestDTO RequestDTO { get; }
}

#endregion
