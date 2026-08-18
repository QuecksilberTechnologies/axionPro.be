// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for updating Policy Type Insurance Mapping.
// ================================================================

using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class UpdatePolicyTypeInsuranceMappingRequestDTO
    {
      
        public int Id { get; set; }
        public int? InsurancePolicyId { get; set; }

        public bool? IsActive { get; set; }

        public int? PolicyTypeId { get; set; }

    }
}
