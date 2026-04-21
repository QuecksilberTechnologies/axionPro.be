using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOs.Manager.ReportingType
{
    public class CreateReportingTypeRequestDTO
    {
        

        public required string TypeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();
     


    }
}
