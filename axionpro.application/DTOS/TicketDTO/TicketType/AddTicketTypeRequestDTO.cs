using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class AddTicketTypeRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int ResponsibleRoleId { get; set; }
        public long? TenantId { get; set; }
        public string TicketTypeName { get; set; } = null!;
        public int TicketHeaderId { get; set; }       
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
       
    }
}
