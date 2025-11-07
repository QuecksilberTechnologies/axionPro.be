using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.Module.ParentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.UserLogin
{

    public class UserResponseDTO
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }
        public GetEmployeeLoginInfoResponseDTO? EmployeeInfo { get; set; }
        public string? Allroles { get; set; }
        public List<ModuleResponseDTO>? CommonItemList { get; set; }
        public List<MainModuleDto>? OperationalMenus { get; set; }

    }
    public class UserInfoDTO
    {
        public int EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }    
        public long? TenantId { get; set; }
        public string? TenantName { get; set; }
        public bool? IsPasswordChangeRequired { get; set; }
        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
        public int EmployeeTypeId { get; set; }
        public string? OfficialEmail { get; set; }
        public string? EmployeeFullName { get; set; }
        public UserRoleDTO? UserPrimaryRole { get; set; }
        public List<UserRoleDTO>? UserSecondryRoles { get; set; }


    }
}
