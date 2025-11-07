using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.Header
{
    public class UpdateHeaderRequestDTO
    {
        public long EmployeeId { get; set; } // EmployeeId of the requester
        public int RoleId { get; set; } // RoleId of the requester
        public long TenantId { get; set; } // TenantId to which the headers belong
        public long Id { get; set; } // The ID of the header to be updated
        public string? HeaderName { get; set; } 
        public int? TicketClassificationId { get; set; } // Foreign key to TicketClassification

        public string? TicketClassificationName { get; set; } // Name of the TicketClassification

        public bool? IsAssetRelated { get; set; }  // Indicates if the header is related to an asset

        public string? Description { get; set; }

        public bool? IsActive { get; set; } 


    }
}
