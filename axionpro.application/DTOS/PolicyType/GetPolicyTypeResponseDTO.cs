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
    public class GetPolicyTypeResponseDTO:BaseRequest
    {
        public int Id { get; set; }
        public long TenantId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedDateTime { get; set; }
        public GetCompanyPolicyDocumentResponseDTO responseDTO { get; set; } = new GetCompanyPolicyDocumentResponseDTO();

    }

}
