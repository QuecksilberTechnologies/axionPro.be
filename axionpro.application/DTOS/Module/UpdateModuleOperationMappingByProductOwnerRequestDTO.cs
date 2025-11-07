using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Module
{
  public  class UpdateModuleOperationMappingByProductOwnerRequestDTO
    {

    public long ProductOwnerId { get; set; }
    public long ProductOwnerRoleId { get; set; }
    public int ModuleOperationMappingId { get; set; }
        public int ModuleId { get; set; }        
        public int OperationId { get; set; }        
        public string? DisplayName { get; set; }
        public string? PageURL { get; set; }
        public string? IconURL { get; set; }
        public bool IsCommonItem { get; set; }
        public bool IsOperational { get; set; }
        public int Priority { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
         
        
    
    }
}
