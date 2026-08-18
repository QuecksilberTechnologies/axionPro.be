// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Reporting Type.
// ================================================================

using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOs.Manager.ReportingType
{
    /// <summary>
    /// Represents the GetReportingTypeRequestDTO data transfer model.
    /// </summary>
    public class GetReportingTypeRequestDTO: BaseRequest
    {
        
      
        public bool? IsActive { get; set; }
        public string? TypeName { get; set; }


    }
}
