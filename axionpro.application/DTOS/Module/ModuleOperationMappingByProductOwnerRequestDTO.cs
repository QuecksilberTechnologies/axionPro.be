using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Module
{
    public class ModuleOperationMappingByProductOwnerRequestDTO
    {
        
        public long ProductOwnerId { get; set; }        
        public long ProductOwnerRoleId { get; set; }
        public int ModuleId { get; set; }         
        public List<Operation> Operation { get; set; }

    }

    public class Operation
    {
        public int Id { get; set; }
    }
}
