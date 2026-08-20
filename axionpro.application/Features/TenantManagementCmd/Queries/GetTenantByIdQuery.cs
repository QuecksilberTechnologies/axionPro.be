// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side query contract for retrieving one Tenant.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Queries;

#region Query

/// <summary>
/// Represents the Host-side request to retrieve one Tenant for details or editing.
/// </summary>
/// <remarks>
/// The future handler must validate the Host with <c>ValidateHostUserRequestAsync()</c> before
/// loading the Tenant. No Host user identifier is accepted from the client.
/// </remarks>
public sealed class GetTenantByIdQuery : IRequest<ApiResponse<TenantResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantByIdQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant identifier request.</param>
    public GetTenantByIdQuery(GetTenantByIdRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant identifier request.
    /// </summary>
    public GetTenantByIdRequestDTO RequestDTO { get; }
}

#endregion
