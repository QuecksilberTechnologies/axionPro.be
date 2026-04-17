using axionpro.application.DTOS.Common;
using axionpro.domain.Entity; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class AddTicketTypeRequestDTO
    {
        // 🔹 Basic Info
        public string TicketTypeName { get; set; } = null!;

        public long TicketHeaderId { get; set; }

        public string? Description { get; set; }

        // 🔹 Responsible Execution
        public int ResponsibleRoleId { get; set; }

        // 🔥 Approval Engine
        public bool IsApprovalRequired { get; set; }

        public int? ApprovalRoleId { get; set; }

        public bool AutoApproveIfSameRole { get; set; }

        // 🔥 SLA
        public int? SLAHours { get; set; }

        // 🔹 Extra Config
        public bool IsActiveForAllUsers { get; set; } = true;

        // 🔹 Common Request (Tumhare pattern ke hisaab se)
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
