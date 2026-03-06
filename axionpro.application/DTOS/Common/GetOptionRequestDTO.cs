using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Common
{
   public class GetOptionRequestDTO
    {
        public string? UserEmployeeId { get; set; }  
        public  DateTime? TodaysDate { get; set; }        
      
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

    }
}
