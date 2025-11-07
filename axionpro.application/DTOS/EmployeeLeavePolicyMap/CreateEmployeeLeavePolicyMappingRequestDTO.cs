using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.EmployeeLeavePolicyMap
{
    public class CreateEmployeeLeavePolicyMappingRequestDTO
    {
       
       
        public long TenantId { get; set; }

        public long EmployeeId { get; set; }
        public long AssignEmployeeId { get; set; }

        public long PolicyLeaveTypeMappingId { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }   // optional banaya, kyunki future me null bhi ho sakta hai

        public bool IsActive { get; set; }          // null aaye to default handle kar lena
      
        public string? Remark { get; set; }

     
    }

}
