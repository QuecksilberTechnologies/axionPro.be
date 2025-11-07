using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Operation
{
    public class GetOperationResponseDTO: BaseRequest
    {
        public int? Id { get; set; } // Nullable

        public string? OperationName { get; set; }  // Default value
        public string? Remark { get; set; } // Nullable
        public bool IsActive { get; set; } = false; // Default false
        public long? AddedById { get; set; } // Nullable
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow; // Default value
        public long? UpdatedById { get; set; } // Nullable
        public DateTime UpdateDateTime { get; set; } = DateTime.UtcNow; // Default value
    }
}
