// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Policy Type Document.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.DTOS.PolicyTypeDocument
{
    /// <summary>
    /// Represents the GetPolicyTypeDocumentRequestDTO data transfer model.
    /// </summary>
    public class GetPolicyTypeDocumentRequestDTO : BaseRequest
    {
        // 🔐 Common decoded props (TenantId, UserEmployeeId etc.)

        // 🔍 Filters
        public long? Id { get; set; }
        public int? PolicyTypeId { get; set; }
        public string? DocumentTitle { get; set; }
        public bool? IsActive { get; set; }

       
    }
}
