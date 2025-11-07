using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Module.SubModule
{
    /// <summary>
    /// Data Transfer Object used for creating or updating a Sub-Module within the system.
    /// </summary>
    public class CreateSubModuleRequestDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the sub-module.
        /// </summary>
        /// <remarks>
        /// This field is optional and mainly used during update operations.
        /// </remarks>
        /// <example>10</example>

        /// <summary>
        /// Gets or sets the ID of the employee who owns or creates the module.
        /// </summary>
        /// <example>101</example>
        public required long EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the tenant under which the module is created.
        /// </summary>
        /// <example>2001</example>
        public required long TenantId { get; set; }

        /// <summary>
        /// Gets or sets the role ID associated with the module.
        /// </summary>
        /// <example>3</example>
        public required int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the unique code for the module.
        /// </summary>
        /// <remarks>Used for internal identification or linking modules.</remarks>
        /// <example>MOD001</example>
        public int? Id { get; set; }

        public string? ModuleCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the sub-module.
        /// </summary>
        /// <remarks>
        /// The name is required and should not exceed 50 characters.
        /// </remarks>
        /// <example>Employee Management</example>
        public string ModuleName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the display name of the sub-module for the UI.
        /// </summary>
        /// <example>Employee Records</example>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the path or route of the sub-module in the application.
        /// </summary>
        /// <example>/employee/records</example>
        public string? URLPath { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent module under which this sub-module falls.
        /// </summary>
        /// <example>1</example>
        public required int ParentModuleId { get; set; }

        /// <summary>
        /// Indicates whether this module is a leaf node (i.e., has no children).
        /// </summary>
        /// <example>true</example>
        public bool? IsLeafNode { get; set; } = true;

        /// <summary>
        /// Indicates whether this module should be displayed in the UI menu.
        /// </summary>
        /// <example>true</example>
        public bool IsModuleDisplayInUi { get; set; } = true;

        /// <summary>
        /// Indicates whether this module is a common menu item shared across all roles.
        /// </summary>
        /// <example>false</example>
        public bool IsCommonMenu { get; set; } = false;

        /// <summary>
        /// Indicates whether this module is active.
        /// </summary>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the web icon path for the module.
        /// </summary>
        /// <example>/images/icons/web/employee.png</example>
        public string? ImageIconWeb { get; set; }

        /// <summary>
        /// Gets or sets the mobile icon path for the module.
        /// </summary>
        /// <example>/images/icons/mobile/employee.png</example>
        public string? ImageIconMobile { get; set; }

        /// <summary>
        /// Gets or sets the item priority for display order.
        /// </summary>
        /// <example>2</example>
        public int? ItemPriority { get; set; }

        /// <summary>
        /// Gets or sets any remarks or notes for this module.
        /// </summary>
        /// <example>Used for managing employee information.</example>
        public string? Remark { get; set; }

    }

}
