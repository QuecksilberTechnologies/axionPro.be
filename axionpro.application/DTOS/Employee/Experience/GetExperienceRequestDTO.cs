// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Experience.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace axionpro.application.DTOS.Employee.Experience
{
    /// <summary>
    /// Represents the GetExperienceRequestDTO data transfer model.
    /// </summary>
    public class GetExperienceRequestDTO : PermissionPagedRequestDTO
    {
        //  Required (encoded)
        public required string EmployeeId { get; set; } = string.Empty;
        public  string? UserEmployeeId { get; set; }

        //  Common Props (Tenant/User context)
    }



}
