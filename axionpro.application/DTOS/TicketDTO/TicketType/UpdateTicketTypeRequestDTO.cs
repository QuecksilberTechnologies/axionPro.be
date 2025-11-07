using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class UpdateTicketTypeRequestDTO
    {
        public long Id { get; set; }             // Existing TicketType Id
        public long EmployeeId { get; set; }     // Id of the employee making the update
        public int RollId { get; set; }     // Id of the employee making the update
        public long? TenantId { get; set; }
        public int? ResponsibleRoleId { get; set; }          // Role of the employee
        public string? TicketTypeName { get; set; }          
        public string? Description { get; set; }
        public bool? IsActive { get; set; } 
        
    }
}
