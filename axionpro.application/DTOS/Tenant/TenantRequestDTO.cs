using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Tenant
{
  
        public class TenantRequestDTO
         {
            public long EmployeeId { get; set; }
            
            public int RoleId { get; set; }
            public int SubscriptionPlanId { get; set; }
            public int DesignationId { get; set; }
            public long? Id { get; set; }
            public string? CompanyName { get; set; } 

           public string? TenantCode { get; set; }

           public string? CompanyEmailDomain { get; set; } 

           public string? TenantEmail { get; set; }    

           public string? ContactPersonName { get; set; }

           public string? ContactNumber { get; set; }

           public int? CountryId { get; set; }

           public bool? IsVerified { get; set; }

           public bool? IsActive { get; set; }


    }

}
