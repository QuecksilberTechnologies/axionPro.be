using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class GetTicketTypeResponseDTO
    {
    public long Id { get; set; }
    public string TicketTypeName { get; set; } = null!;
    public long? TicketHeaderId { get; set; }
     public int? ResponsibleRoleId { get; set; }
     public long? ResponsibleEmployeeId { get; set; }
     public string? ResponsibleEmployeeEmailId { get; set; }
     public string? ResponsibleEmployeeName { get; set; }
     public string? ResponsibleRoleName{ get; set; }
     public long? TenantId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }    
    public long? AddedById { get; set; }
    public DateTime AddedDateTime { get; set; }
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
 
    }

    public class GetTicketTypeRoleResponseDTO
    {
      
        public long Id { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ResponsibleRoleId { get; set; }
        public string ResponsibleRoleName { get; set; } = string.Empty;
        public List<EmployeeShortInfoDTO> Employees { get; set; } = new();
    }

    public class EmployeeShortInfoDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
