// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for updating Employee Section Status.
// ================================================================

using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{

    
    /// <summary>
    /// Represents the UpdateEmployeeSectionStatusRequestDTO data transfer model.
    /// </summary>
    public class UpdateEmployeeSectionStatusRequestDTO : axionpro.application.DTOs.BaseDTO.PermissionRequestDTO
    {
       
        public required string EmployeeId { get; set; }   
        public bool? IsActive { get; set; }


        public List<SectionStatusDTO>? Sections { get; set; }
    }

    /// <summary>
    /// Represents the SectionStatusDTO data transfer model.
    /// </summary>
    public class SectionStatusDTO
    {
        public required int TabInfoType { get; set; } // "education", "bank", "experience"
        public bool? IsVerified { get; set; }
        public bool? IsEditAllowed { get; set; }

        // Optional: Primary key for that section (EducationId, BankId)
      //   public string? EmployeeId { get; set; }
    
    
    }

}
