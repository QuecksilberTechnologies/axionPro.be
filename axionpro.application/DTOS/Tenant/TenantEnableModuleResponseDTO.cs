using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Tenant
{
    public class GetModuleHierarchyResponseDTO
    {
        public long? TenantId { get; set; }
        public List<ModuleNodedto> Modules { get; set; } = new();
    }

    public class ModuleNodedto
    {
        public long Id { get; set; }
        public string? ModuleName { get; set; }
        public long? ParentModuleId { get; set; }
        public bool IsEnabled { get; set; }

        public List<ModuleNodedto> Children { get; set; } = new();
    }

}
