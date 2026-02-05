using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class CreatePolicyTypeInsuranceMappingRequetDTO
    {       
        public int InsurancePolicyId { get; set; }

        public bool IsActive { get; set; }

        public int PolicyTypeId { get; set; }  
        
        public ExtraPropRequestDTO? Props { get; set; } = new ExtraPropRequestDTO();


    }
}
