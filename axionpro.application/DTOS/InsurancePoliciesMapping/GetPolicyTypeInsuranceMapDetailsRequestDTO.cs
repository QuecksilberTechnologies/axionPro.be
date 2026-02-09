using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class GetPolicyTypeInsuranceMapDetailsRequestDTO
    {
      
        public int PolicyTypeId { get; set; }
        public int InsuranceTypeId { get; set; }       
        public bool IsActive { get; set; }         

    }

}
