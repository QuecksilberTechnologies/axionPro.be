
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.ReportTypeCmd.Commands
{
    public class DeleteReportingTypeCommand : IRequest<ApiResponse<bool>>
    {

        public DeleteReportingTypeRequestDTO? DTO { get; set; }

        public DeleteReportingTypeCommand(DeleteReportingTypeRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

}
