using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.ProjectModule;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.DTOS.StoreProcedures.DashboardSummeries;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IStoreProcedureRepository
    {
        Task<long> ValidateActiveUserLoginOnlyAsync(string loginId);
        Task<long> ValidateActiveUserCrendentialOnlyAsync(string loginId);

        Task<int> ValidateUserPasswordAsync(string loginId);
        Task<bool> UpdateLoginCredential(LoginRequestDTO loginId);
       

        Task<UpdateTenantEnabledOperationFromModuleOperationResponseDTO> UpdateTenantEnabledOperationFromModuleOperationRequestDTO(
            UpdateTenantEnabledOperationFromModuleOperationRequestDTO request);

        Task<List<RoleModuleOperationResponseDTO>> GetActiveRoleModuleOperationsAsync( GetActiveRoleModuleOperationsRequestDTO request);
        Task<List<GetModuleOperationRolePermissionsResponseDTO>> GetTenantModulesConfigurationResponses(GetTenantModuleOperationRolePermissionsRequestDTO request);
         

        Task<List<SubscribedModuleResponseDTO>> GetSubscribedModulesByTenantAsync(long tenantId);
        Task<bool> GetHasAccessOperation(GetCheckOperationPermissionRequestDTO checkOperationPermissionRequest);
        Task<bool> HasPermissionAsync(long userId, string permissionCode);
        Task<bool> IsTenantValidAsync(long userId, long? TenantId);

        Task<List<GetEmployeeIdentitySp>> GetIdentityRecordAsync(long employeeId, int countryId, bool isActive);
        Task<EmployeeCountResponseStatsSp> GetEmployeeCountsAsync(long tenantId);

        //   Task  <IUserRoleRepository> UpdateLoginCredential(LoginRequestDTO loginId);
        //  Task List<string> UpdateLoginCredential(LoginRequestDTO loginId);

    }
}
 