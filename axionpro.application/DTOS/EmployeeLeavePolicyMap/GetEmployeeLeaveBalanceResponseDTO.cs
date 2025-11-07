using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.EmployeeLeavePolicyMap
{
    public class GetEmployeeLeaveBalanceResponseDTO: BaseRequest
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public long? EmployeeLeavePolicyMappingId { get; set; }
        public int? LeaveYear { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? Availed { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? CarryForwarded { get; set; }
        public decimal? Encashed { get; set; }
        public decimal? LeavesOnHold { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAllBalanceOnHold { get; set; }
        public long? AddedById { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    
    }

}
