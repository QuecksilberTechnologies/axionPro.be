using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class GetTicketTypeResponseDTO
    {
        public long Id { get; set; }
        public string TicketTypeName { get; set; } = null!;

        public long? TicketHeaderId { get; set; }
        public string? TicketHeaderName { get; set; }   // 🔥 UI ke liye

        public long? TenantId { get; set; }

        // 🔥 Responsible Role
        public int? ResponsibleRoleId { get; set; }
        public string? ResponsibleRoleName { get; set; }

        // 🔥 (Optional UI enrichment - keep if needed)
        public long? ResponsibleEmployeeId { get; set; }
        public string? ResponsibleEmployeeEmailId { get; set; }
        public string? ResponsibleEmployeeName { get; set; }

        // 🔥 Approval Engine (NEW)
        public bool? IsApprovalRequired { get; set; }
        public int? ApprovalRoleId { get; set; }
        public string? ApprovalRoleName { get; set; }
        public bool? AutoApproveIfSameRole { get; set; }

        // 🔥 SLA
        public int? SLAHours { get; set; }

        // 🔥 Extra Config
        public bool? IsActiveForAllUsers { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

 
    }
    public class GetTicketTypeRoleResponseDTO
    {
        public long Id { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int ResponsibleRoleId { get; set; }
        public string ResponsibleRoleName { get; set; } = string.Empty;

        // 🔥 Approval info bhi add karo (useful for UI)
        public bool? IsApprovalRequired { get; set; }
        public int? ApprovalRoleId { get; set; }
        public string? ApprovalRoleName { get; set; }

        public List<EmployeeMinInfoDTO> Employees { get; set; } = new();
    }
    public class EmployeeMinInfoDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
