using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.EmployeeLeavePolicyMap
{
    public class UpdateEmployeeLeavePolicyMappingRequestDTO
    {
        public long PolicyLeaveTypeMappingId { get; set; }
        public long TenantId { get; set; }    
        public long EmployeeId { get; set; }      
        public long AssignEmployeeId { get; set; }      
        public bool IsActive { get; set; }          
 
    }
}
