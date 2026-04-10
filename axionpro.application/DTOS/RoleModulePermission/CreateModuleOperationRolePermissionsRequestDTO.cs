using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOs.RoleModulePermission
{
  
    public class CreateModuleOperationRolePermissionsRequestDTO
    {
        public int RoleId { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();

        public List<ModulePermissionDTO>? ModuleOperations { get; set; }
    }

    

     


}
