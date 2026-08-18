// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Ticket Header By Classify Id.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace axionpro.application.DTOS.TicketDTO.Header
{
    public class GetTicketHeaderByClassifyIdRequestDTO
    {
   
        public int TicketClassifyId { get; set; }
       
    }
}
