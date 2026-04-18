using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOS.TicketDTO.Header
{
    public class AddHeaderRequestDTO
    {

       

        public string HeaderName { get; set; } = null!;

        public string? TicketClassificationId { get; set; }

        public string? TicketClassificationName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public ExtraPropRequestDTO  Prop { get; set; } = new ExtraPropRequestDTO(); 







    }
}
