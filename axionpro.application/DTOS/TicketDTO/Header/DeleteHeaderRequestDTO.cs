using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.Header
{
    public class DeleteHeaderRequestDTO
 {
         public long Id { get; set; }// The ID performing  deletion
        public long EmployeeId { get; set; } // EmployeeId of the requester
        public int RoleId { get; set; } // RoleId of the requester
        public long TenantId { get; set; } // TenantId to which the headers belong
    }
}