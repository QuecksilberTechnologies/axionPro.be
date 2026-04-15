using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    public class GetClassificationRequestDTO : BaseRequest
    {

        public int Id { get; set; }  
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();



    }
}
