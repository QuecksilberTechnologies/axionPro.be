using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Tenant
{

    public class TenantSubscriptionPlanRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long TenantId { get; set; }
        public int? Id { get; set; }      
        public int? SubscriptionPlanId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTrial { get; set; }


    }
}
