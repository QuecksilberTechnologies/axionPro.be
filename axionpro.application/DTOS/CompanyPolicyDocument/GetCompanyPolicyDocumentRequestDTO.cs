using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.CompanyPolicyDocument
{
    public class GetCompanyPolicyDocumentRequestDTO : BaseRequest
    {
        // 🔐 Common decoded props (TenantId, UserEmployeeId etc.)
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

        // 🔍 Filters
        public long? Id { get; set; }
        public int? PolicyTypeId { get; set; }
        public string? DocumentTitle { get; set; }
        public bool? IsActive { get; set; }

       
    }
}
