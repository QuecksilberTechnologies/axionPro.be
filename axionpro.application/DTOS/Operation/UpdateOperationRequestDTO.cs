using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Operation
{
    /// <summary>
    /// post-request to update operation
    /// </summary>
    public class UpdateOperationRequestDTO
    {
        /// <summary> Product Id Required</summary>

        public required int ProductOwnerId { get; set; }
        /// <summary> Product Owner Role Id Id Required</summary>

        public required int ProductOwnerRoleId { get; set; }
        public required int Id { get; set; } // Nullable
        public  int?  OperationType { get; set; } // Nullable
        public  string? OperationName { get; set; }  // Default value
        public string? Remark { get; set; } // Nullable
        public string? IconImage { get; set; } // Nullable
        public bool? IsActive { get; set; } = false; // Default false
     

    }
}
