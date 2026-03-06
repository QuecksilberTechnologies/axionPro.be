using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.Tenant
{
    public class PlanModuleMappingRequestDTO
     {
        
            public long? TenantId { get; set; }
            public long EmployeeId { get; set; }
            public int SubscriptionPlanId { get; set; }            
            public bool? IsActive { get; set; }
            public string? Remark { get; set; }
         
        }
  
}
