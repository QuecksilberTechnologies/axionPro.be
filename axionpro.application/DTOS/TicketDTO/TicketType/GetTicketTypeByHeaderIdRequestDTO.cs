using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class GetTicketTypeByHeaderIdRequestDTO 
    {
        public long TenantId { get; set; }
        public long Id { get; set; }
        public long EmployerId { get; set; }
        public int RoleId { get; set; }
     
        public int? TicketHeaderId { get; set; }
    }
}
