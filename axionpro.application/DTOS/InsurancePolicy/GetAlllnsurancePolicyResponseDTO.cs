using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class GetAlllnsurancePolicyResponseDTO
    {
        // 🔹 Identity
        public int InsurancePolicyId { get; set; }

        // 🔹 Policy Type
        public int PolicyTypeId { get; set; }
        public string InsurancePolicyName { get; set; } = string.Empty;
    }
}
