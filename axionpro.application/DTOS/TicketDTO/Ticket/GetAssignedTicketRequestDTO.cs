using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.TicketDTO.Ticket
{
   
        public class GetAssignedTicketRequestDTO : BaseRequest
        {
        public required int  Status { get; set; }   // optional (Open, Pending etc.)

        // 🔥 Login user info (CommonRequestService se aayega)
        public ExtraPropRequestDTO? Prop { get; set; } = new();

            // 🔥 Filter
        }
     
}
