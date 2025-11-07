using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class GetTicketTypeByIdRequestDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long? TenantId { get; set; }
       
     
    }
}
