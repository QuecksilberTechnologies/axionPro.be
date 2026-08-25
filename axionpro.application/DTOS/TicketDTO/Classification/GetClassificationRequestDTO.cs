using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    public class GetClassificationRequestDTO : PermissionRequestDTO
    {

        public int Id { get; set; }  
         


    }
}
