using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.Header
{
    public class AddHeaderRequestDTO
    {
        public long EmployeeId { get; set; } // EmployeeId of the requester
        public int RoleId { get; set; } // RoleId of the requester
        public long TenantId { get; set; } // TenantId to which the headers belong

        public string HeaderName { get; set; } = null!;

        public int TicketClassificationId { get; set; }

        public bool IsAssetRelated { get; set; } 

        public string? Description { get; set; }

        public bool IsActive { get; set; } =true;

       





    }
}
