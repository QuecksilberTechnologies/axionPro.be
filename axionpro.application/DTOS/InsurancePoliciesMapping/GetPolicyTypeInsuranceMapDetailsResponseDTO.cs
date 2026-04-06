using axionpro.application.DTOS.Pagination;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class GetPolicyTypeInsuranceMapDetailsResponseDTO
    {
        public int Id { get; set; }        
        public int PolicyTypeId { get; set; }
        public int InsuranceTypeId { get; set; }
        public int MaxChildAllowed { get; set; }
        public int MaxSpouseAllowed { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string InsuranceTypeName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;       
        public string? Description { get; set; }
        public bool IsActive { get; set; }  
        public bool ParentsAllowed { get; set; }
        public bool InLawsAllowed { get; set; }




    }

}
