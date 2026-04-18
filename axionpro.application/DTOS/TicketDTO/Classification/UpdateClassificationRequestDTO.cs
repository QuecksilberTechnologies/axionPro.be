using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    public class UpdateClassificationRequestDTO
    {
  
    public int  Id { get; set; }
    public string? ClassificationName { get; set; } 
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();
   
}
}
