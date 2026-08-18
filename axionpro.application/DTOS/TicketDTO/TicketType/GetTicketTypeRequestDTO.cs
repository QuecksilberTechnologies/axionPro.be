// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Ticket Type.
// ================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class GetTicketTypeRequestDTO:BaseRequest
    {


    }
    public class GetDDLTicketTypeRequestDTO
    {

        public bool IsActive { get; set; }


    }

}
