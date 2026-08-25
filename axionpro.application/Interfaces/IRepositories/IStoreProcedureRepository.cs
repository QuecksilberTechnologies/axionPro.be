// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines stored-function and validated runtime authorization read operations.
// ================================================================

using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.DTOS.StoreProcedures.DashboardSummeries;
using axionpro.application.DTOS.Tenant;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IStoreProcedureRepository
    {
        Task<long> ValidateActiveUserLoginOnlyAsync(string loginId);
        Task<long> ValidateActiveUserCrendentialOnlyAsync(string loginId);

        Task<int> ValidateUserPasswordAsync(string loginId);
        Task<bool> UpdateLoginCredential(LoginRequestDTO loginId);

        /// <summary>
        /// Checks whether the current Tenant employee can execute the requested
        /// module operation using the employee's current Primary and Secondary roles.
        /// </summary>
        /// <param name="tenantId">Authenticated Tenant identifier.</param>
        /// <param name="employeeId">Authenticated Employee identifier.</param>
        /// <param name="tokenRoleId">Primary Role identifier contained in the current JWT.</param>
        /// <param name="moduleId">Requested Module identifier.</param>
        /// <param name="operationId">Requested Operation identifier.</param>
        /// <param name="cancellationToken">Token used to cancel the database operation.</param>
        /// <returns>
        /// The current authorization result including stale-role detection and
        /// the Role that granted access.
        /// </returns>
        Task<TenantsUserPermissionCheckResponseDTO> CheckTenantEmployeePermissionAsync(
                long tenantId,
                long employeeId,
                int tokenRoleId,
                int moduleId,
                int operationId,
                CancellationToken cancellationToken = default);

        Task<UpdateTenantEnabledOperationFromModuleOperationResponseDTO> UpdateTenantEnabledOperationFromModuleOperationRequestDTO(
            UpdateTenantEnabledOperationFromModuleOperationRequestDTO request);

        /// <summary>
        /// Retrieves the legacy Tenant operational permission rows for a supplied internal role-ID set.
        /// </summary>
        /// <param name="request">The trusted Tenant identifier and internal comma-separated role identifiers.</param>
        /// <returns>The legacy operational permission rows.</returns>
        Task<List<RoleModuleOperationResponseDTO>> GetActiveRoleModuleOperationsAsync(
            GetActiveRoleModuleOperationsRequestDTO request);

        /// <summary>
        /// Retrieves the authenticated tenant employee's current effective module-operation access
        /// using the authoritative tenant permission source.
        /// </summary>
        /// <param name="tenantId">The validated Tenant identifier.</param>
        /// <param name="roleIds">The current effective role identifiers resolved by the application.</param>
        /// <param name="cancellationToken">A token used to cancel the database operation.</param>
        /// <returns>The current permitted operational rows used to construct navigation.</returns>
        Task<List<RoleModuleOperationResponseDTO>> GetCurrentTenantOperationalAccessAsync(
            long tenantId,
            IReadOnlyCollection<int> roleIds,
            CancellationToken cancellationToken = default);
        Task<List<GetModuleOperationRolePermissionsResponseDTO>> GetTenantModulesConfigurationResponses(GetTenantModuleOperationRolePermissionsRequestDTO request);
         

        Task<List<SubscribedModuleResponseDTO>> GetSubscribedModulesByTenantAsync(long tenantId);
        Task<bool> GetHasAccessOperation(GetCheckOperationPermissionRequestDTO checkOperationPermissionRequest);
        Task<bool> HasPermissionAsync(long userId, string permissionCode);
        Task<bool> IsTenantValidAsync(long userId, long? TenantId);

        Task<List<GetEmployeeIdentitySp>> GetIdentityRecordAsync(long employeeId, int countryId, bool isActive);
        Task<EmployeeCountResponseStatsSp> GetEmployeeCountsAsync(long tenantId);
        Task<GetEmployeeCodePatternResponseDTO?> GetTenantEmployeeCodePatternAsync(
             EmployeeCodePatternRequestDTO request);

        //   Task  <IUserRoleRepository> UpdateLoginCredential(LoginRequestDTO loginId);
        //  Task List<string> UpdateLoginCredential(LoginRequestDTO loginId);

    }
}

namespace axionpro.application.DTOS.StoreProcedures
{
    public class TenantUserPermissionCheckResponseDTO
    {
        // add properties according to your domain model
    }
}
