// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Employee Summary.
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
    /// Represents the GetEmployeeSummaryRequestDTO data transfer model.
    /// </summary>
    public class GetEmployeeSummaryRequestDTO : axionpro.application.DTOs.BaseDTO.PermissionRequestDTO
    {
        public required string EmployeeId { get; set; }
        public required bool IsActive { get; set; }
    }
}
