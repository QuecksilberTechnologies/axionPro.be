// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Employee Image.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
 
    /// <summary>
    /// Represents the GetEmployeeImageRequestDTO data transfer model.
    /// </summary>
    public class GetEmployeeImageRequestDTO 
    {

        public string? EmployeeId { get; set; } 
        public bool IsActive { get; set; } =true;


    }
}
