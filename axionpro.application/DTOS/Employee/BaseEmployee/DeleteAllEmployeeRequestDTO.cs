// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for deleting All Employee.
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
    /// Represents the DeleteBaseEmployeeRequestDTO data transfer model.
    /// </summary>
    public class DeleteBaseEmployeeRequestDTO
    {
      
        
        public required string EmployeeId { get; set; } = string.Empty;


    }

}
