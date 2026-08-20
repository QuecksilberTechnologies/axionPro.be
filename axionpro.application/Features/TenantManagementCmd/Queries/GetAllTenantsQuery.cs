// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side query contract for Tenant management records.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Queries;

#region Query

/// <summary>
/// Represents the Host-side request to retrieve Tenant management records.
/// </summary>
/// <remarks>
/// The future handler must validate the Host with <c>ValidateHostUserRequestAsync()</c> before
/// returning Tenant management data. No Host user identifier is accepted from the client.
/// </remarks>
public sealed class GetAllTenantsQuery : IRequest<ApiResponse<List<TenantResponseDTO>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllTenantsQuery"/> class.
    /// </summary>
    /// <param name="requestDTO">The optional Tenant management filters and paging values.</param>
    public GetAllTenantsQuery(GetAllTenantsRequestDTO? requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the optional Tenant management filters and paging values.
    /// </summary>
    public GetAllTenantsRequestDTO? RequestDTO { get; }
}

#endregion
