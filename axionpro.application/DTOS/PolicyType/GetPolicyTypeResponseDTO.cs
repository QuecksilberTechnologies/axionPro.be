using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    public class GetPolicyTypeResponseDTO
    {
        public int Id { get; set; }
        public long TenantId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }   
        public bool IsMappedWithInsurance { get; set; } = false;
        public GetCompanyPolicyDocumentResponseDTO DocDetails { get; set; } = new GetCompanyPolicyDocumentResponseDTO();

    }

}
