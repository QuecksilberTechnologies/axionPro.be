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
    public class GetAllReportingTypeQuery : IRequest<ApiResponse<List<GetReportingTypeResponseDTO>>>
    {
        public GetReportingTypeRequestDTO DTO { get; set; }

    public GetAllReportingTypeQuery(GetReportingTypeRequestDTO dTO)
    {
        this.DTO = dTO;
    }
}
}
