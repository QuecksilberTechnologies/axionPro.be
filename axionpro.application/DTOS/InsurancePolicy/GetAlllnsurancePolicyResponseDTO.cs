using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class GetAlllnsurancePolicyResponseDTO
    {
        // 🔹 Identity
        public int InsurancePolicyId { get; set; }

        // 🔹 Policy Type
        public int PolicyTypeId { get; set; }
        public string InsurancePolicyName { get; set; } = string.Empty;

    

         public class GetAlllnsurancePolicyWithDetailsResponseDTO
    {
        // 🔹 Identity
        public int InsurancePolicyId { get; set; }

        // 🔹 Policy Type
        public int PolicyTypeId { get; set; }
        public string InsurancePolicyName { get; set; } = string.Empty;
        public bool? IsEmployeeConsumed { get; set; }
        public bool? IsDependentConsumed { get; set; }
        public int ConsumedDependentCount { get; set; } = 0;
    }
}
}
