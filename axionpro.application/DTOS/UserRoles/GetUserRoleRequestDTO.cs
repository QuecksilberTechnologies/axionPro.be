// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving User Role.
// ================================================================

using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.UserRoles
{
    /// <summary>
    /// Represents the GetUserRoleRequestDTO data transfer model.
    /// </summary>
    public class GetUserRoleRequestDTO
    {
        public required string EmployeeId { get; set; }
    }
}
