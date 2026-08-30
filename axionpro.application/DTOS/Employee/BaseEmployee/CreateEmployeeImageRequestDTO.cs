// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for creating Employee Image.
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
    /// Represents the CreateEmployeeImageRequestDTO data transfer model.
    /// </summary>
    public class CreateEmployeeImageRequestDTO : axionpro.application.DTOs.BaseDTO.PermissionRequestDTO
    {
         
        public required bool IsActive { get; set; }      
        public IFormFile? ImageFile { get; set; }

    }
}
