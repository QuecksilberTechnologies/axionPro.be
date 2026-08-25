// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for creating Reporting Type.
// ================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOs.BaseDTO;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOs.Manager.ReportingType
{
    /// <summary>
    /// Represents the CreateReportingTypeRequestDTO data transfer model.
    /// </summary>
    public class CreateReportingTypeRequestDTO : PermissionRequestDTO
    {
        

        public required string TypeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
     


    }
}
