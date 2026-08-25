// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Represents the result of the current Tenant employee
//           runtime module-operation authorization check.
// ================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.RoleModulePermission
{
    #region Tenant Permission Check Response

    /// <summary>
    /// Represents the current Tenant employee authorization result returned
    /// by the PostgreSQL permission function.
    /// </summary>
    public sealed class TenantsUserPermissionCheckResponseDTO
    {
        /// <summary>
        /// Gets or sets the authorization result code.
        /// </summary>
        public int ResultCode { get; set; }

        /// <summary>
        /// Gets or sets the machine-readable authorization result key.
        /// </summary>
        public string ResultKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the employee's current active Primary Role identifier.
        /// </summary>
        public int? CurrentPrimaryRoleId { get; set; }

        /// <summary>
        /// Gets or sets the current Primary or Secondary Role that granted
        /// the requested module-operation permission.
        /// </summary>
        public int? GrantedRoleId { get; set; }
    }
}
#endregion