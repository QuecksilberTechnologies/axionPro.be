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
       
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
