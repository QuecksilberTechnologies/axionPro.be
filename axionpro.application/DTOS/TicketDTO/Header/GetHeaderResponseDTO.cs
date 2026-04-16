using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.TicketDTO.Header
{
    public class GetHeaderResponseDTO : BaseRequest
    {

       
            public string HeaderId { get; set; } = null!;  // 🔐 encoded string (as per your rule)

            public string HeaderName { get; set; } = null!;

            public string? TicketClassificationId { get; set; }

            public string? TicketClassificationName { get; set; }

            public string? Description { get; set; }

            public bool IsActive { get; set; }
        





    }
}
