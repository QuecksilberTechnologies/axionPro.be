// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Dependent.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.Dependent
{
    /// <summary>
    /// Represents the GetDependentRequestDTO data transfer model.
    /// </summary>
    public class GetDependentRequestDTO : PermissionPagedRequestDTO
    {

 
        public required string EmployeeId { get; set; }


    }


}
