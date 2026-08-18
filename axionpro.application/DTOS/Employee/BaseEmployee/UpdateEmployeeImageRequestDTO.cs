// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for updating Employee Image.
// ================================================================

using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    /// <summary>
    /// Represents the UpdateEmployeeImageRequestDTO data transfer model.
    /// </summary>
    public class UpdateEmployeeImageRequestDTO
    {
        
        public long Id { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? FileName { get; set; }
        public required bool IsActive { get; set; }
        

    }
}
