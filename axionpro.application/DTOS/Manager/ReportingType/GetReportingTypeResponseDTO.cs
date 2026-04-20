using axionpro.application.DTOS.Pagination;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.Manager.ReportingType
{
    public class GetReportingTypeResponseDTO: BaseResponse
    {
    public int Id { get; set; }


        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Common audit fields
   
    }
}
