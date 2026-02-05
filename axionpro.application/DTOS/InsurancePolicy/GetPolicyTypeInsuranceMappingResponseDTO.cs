using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class GetPolicyTypeInsuranceMappingResponseDTO
    {
      
        public int PolicyTypeId { get; set; }
        public int? InsurancePolicyId { get; set; }
        public string? InsurancePolicyName { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public string? ProviderName { get; set; }
        public string? PolicyName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
