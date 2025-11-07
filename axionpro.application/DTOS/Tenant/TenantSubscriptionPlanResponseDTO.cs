using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Tenant
{
    public class TenantSubscriptionPlanResponseDTO
    {

        public long Id { get; set; }
        public long? TenantId { get; set; }       
        public int SubscriptionPlanId { get; set; }       
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public bool IsActive { get; set; }
        public string? PaymentTxnId { get; set; }
        public string? PaymentMode { get; set; }
        public bool IsTrial { get; set; }   





    }


}
