// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Policy Type Insurance Mapping.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class GetPolicyTypeInsuranceMappingRequestDTO :BaseRequest
    {
      
        public int? PolicyTypeId { get; set; }

        public int? InsurancePolicyId { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}
