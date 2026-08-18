// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for processing Activate All Employee.
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
    /// Represents the ActivateAllEmployeeRequestDTO data transfer model.
    /// </summary>
    public class ActivateAllEmployeeRequestDTO
    {
        public string? UserEmployeeId { get; set; }
        public string? EmployeeId { get; set; }
        public long? Id { get; set; }
        public required bool IsActive { get; set; }
    }

}
