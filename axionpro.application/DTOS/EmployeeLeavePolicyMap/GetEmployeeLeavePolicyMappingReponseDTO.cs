using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.EmployeeLeavePolicyMap
{
    public class GetEmployeeLeavePolicyMappingReponseDTO: BaseRequest
    {
        public long Id { get; set; }
        public long TenantId { get; set; }

        public long EmployeeId { get; set; }

        public long PolicyLeaveTypeMappingId { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }   // optional banaya, kyunki future me null bhi ho sakta hai

        public bool IsActive { get; set; }          // null aaye to default handle kar lena
        public bool? IsLeaveBalanceAssigned { get; set; }          // null aaye to default handle kar lena

        public string? LeaveName { get; set; }
        public int? LeaveTypeId { get; set; }
       public long AddedById { get; set; }  
       public DateTime AddedDateTime { get; set; }  
    }
}
