using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.domain.Entity;

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
    public class ModuleDTO
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string? URLPath { get; set; }
        public string? DisplayName { get; set; }
        public bool? IsLeafNode { get; set; }

        public string? ImageIconWeb { get; set; }
        public string? ImageIconMobile { get; set; }
        public int? ItemPriority { get; set; }
        public List<ModuleDTO> Children { get; set; } = new();
    }

}
