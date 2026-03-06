using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class CreatePolicyTypeInsuranceMappingRequetDTO
    {       
        public int InsurancePolicyId { get; set; }

        public bool IsActive { get; set; }

        public int PolicyTypeId { get; set; }  
        
        public ExtraPropRequestDTO? Props { get; set; } = new ExtraPropRequestDTO();


    }
}
