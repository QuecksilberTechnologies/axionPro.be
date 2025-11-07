using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Operation
{
    /// <summary>
    /// post-request to insert operation
    /// </summary>
    public class CreateOperationRequestDTO
    {

        /// <summary> Product Owner Id Required</summary>

        public required int  ProductOwnerId { get; set; }
      
        public  int ProductOwnerRoleId { get; set; }
        /// <summary> Operation Name Required</summary>

        public required string OperationName { get; set; }  
        /// <summary> Operation Type Required</summary>
        public required int OperationType { get; set; }   
        /// <summary> Operation Remark Required</summary>

        public required string? Remark { get; set; } // Nullable
        public required bool? IsActive { get; set; } = false; 
        public  string? IconImage { get; set; }  
 
    }
}
