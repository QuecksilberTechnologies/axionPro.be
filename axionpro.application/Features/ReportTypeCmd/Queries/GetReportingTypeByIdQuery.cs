using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.ReportTypeCmd.Queries
{
    public class GetReportingTypeByIdQuery : IRequest<ApiResponse<GetReportingTypeResponseDTO>>
    {
        public GetReportingTypeByIdRequestDTO DTO { get; set; }

    public GetReportingTypeByIdQuery(GetReportingTypeByIdRequestDTO dTO)
    {
        this.DTO = dTO;
    }
}
}
