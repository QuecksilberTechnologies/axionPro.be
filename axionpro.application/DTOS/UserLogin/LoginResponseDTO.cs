using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.ProjectModule;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserLogin
{


    public class LoginResponseDTO
    {
                   
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }        
        public GetEmployeeLoginInfoResponseDTO? EmployeeInfo { get; set; }
        public string? Allroles { get; set; }
        public List<ModuleDTO>? CommonItems { get; set; }
        public List<MainModuleDto>? OperationalMenus { get; set; }
        




    }
}
