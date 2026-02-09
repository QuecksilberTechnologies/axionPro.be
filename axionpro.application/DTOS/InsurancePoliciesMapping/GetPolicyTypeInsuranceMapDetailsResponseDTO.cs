using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class GetPolicyTypeInsuranceMapDetailsResponseDTO
    {
        public int Id { get; set; }        
        public int PolicyTypeId { get; set; }
        public int InsuranceTypeId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string InsuranceTypeName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }  

       

    }

}
