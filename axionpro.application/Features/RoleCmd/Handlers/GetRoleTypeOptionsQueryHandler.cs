// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Publishes the centralized Tenant role types for UI controls.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Role;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents an authenticated request for the centralized Tenant role-type options.
    /// </summary>
    public sealed class GetRoleTypeOptionsQuery
        : IRequest<ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>>
    {
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles role-type option requests without reading tenant data or changing persistence.
    /// </summary>
    public sealed class GetRoleTypeOptionsQueryHandler
        : IRequestHandler<
            GetRoleTypeOptionsQuery,
            ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>>
    {
        /// <summary>
        /// Returns all supported role types from the shared role constants.
        /// </summary>
        public Task<ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>> Handle(
            GetRoleTypeOptionsQuery request,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<RoleTypeOptionResponseDTO> options =
            [
                new()
                {
                    Id = ConstantValues.RoleTypeAdmin,
                    Name = ConstantValues.TenantAdminRoleOptionName,
                    Description = ConstantValues.TenantAdminRoleDescription
                },
                new()
                {
                    Id = ConstantValues.RoleTypeEmployee,
                    Name = ConstantValues.TenantEmployeeRoleDisplayName,
                    Description = ConstantValues.TenantEmployeeRoleDescription
                },
                new()
                {
                    Id = ConstantValues.RoleTypeManager,
                    Name = ConstantValues.TenantManagerRoleDisplayName,
                    Description = ConstantValues.TenantManagerRoleDescription
                },
                new()
                {
                    Id = ConstantValues.RoleTypeClient,
                    Name = ConstantValues.TenantExternalRoleDisplayName,
                    Description = ConstantValues.TenantExternalRoleDescription
                }
            ];

            return Task.FromResult(
                ApiResponse<IReadOnlyList<RoleTypeOptionResponseDTO>>.Success(
                    options,
                    AppConstants.SuccessMessages.RoleOptionsRetrieved));
        }
    }

    #endregion
}
