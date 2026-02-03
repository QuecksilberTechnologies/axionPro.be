using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class CreatePolicyTypeInsuranceMappingDTO
    {
      
        public int PolicyTypeId { get; set; }

        public int InsurancePolicyId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
