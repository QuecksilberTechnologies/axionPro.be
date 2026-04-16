using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOS.TicketDTO.Creation
{
    public class CreateTicketRequestDTO
    {
        public int TicketClassificationId { get; set; }

        public long TicketHeaderId { get; set; }

        public long TicketTypeId { get; set; }

        public string? Description { get; set; }

        public int Priority { get; set; }

        public long RequestedForUserId { get; set; }

        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }

}
