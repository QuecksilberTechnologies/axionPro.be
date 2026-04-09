using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOs.RoleModulePermission
{
  
    public class CreateModuleOperationRolePermissionsRequestDTO
    {
        public int RoleId { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();

        public List<ModulePermissionDTO> ModuleOperations { get; set; }
    }

    public class ModulePermissionDTO
    {
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }

        public List<OperationPermissionDTO> Operations { get; set; } = new();
    }

    public class OperationPermissionDTO
    {
        public int OperationId { get; set; }
        public string? OperationName { get; set; }

        public int OperationType { get; set; } // CRUD / Workflow / Status

        public bool HasAccess { get; set; } // 🔥 checkbox value
    }


}
