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
    public class GetUserRoleRequestDTO
    {
        public required string EmployeeId { get; set; }
    }
}
