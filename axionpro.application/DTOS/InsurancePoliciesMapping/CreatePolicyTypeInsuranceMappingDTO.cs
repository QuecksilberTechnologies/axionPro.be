// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Create Policy Type Insurance Mapping DTO data transfer model.
// ================================================================

using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    /// <summary>
    /// Represents the CreatePolicyTypeInsuranceMappingRequetDTO data transfer model.
    /// </summary>
    public class CreatePolicyTypeInsuranceMappingRequetDTO
    {       
        public int InsurancePolicyId { get; set; }

        public bool IsActive { get; set; }

        public int PolicyTypeId { get; set; }  
        


    }
}
