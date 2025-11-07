using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Registration
{
    public class TenantRequestDTO
    {
       
        public int SubscriptionPlanId { get; set; } 

        public int TenantIndustryId { get; set; } 
        public string CompanyName { get; set; } = null!;

        public string TenantCode { get; set; } = null!;
 
        public string CompanyEmailDomain { get; set; } = null!;
        public int GenderId { get; set; } 
         
        public string TenantEmail { get; set; } = null!;

        public string? ContactPersonName { get; set; }

        public string? ContactNumber { get; set; }
      
        public int CountryId { get; set; }     
 
 
    }

}
