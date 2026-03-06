using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.SubscriptionModule
{
    public class GetTenantSubscriptionDetailResponsDTO 
    {
        public long? TenantId { get; set; }
        public string? PlanName { get; set; }
        public int? TenantSubscriptionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

    }
}
