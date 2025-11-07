using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.ReportTypeCmd.Commands
{
    public class UpdateReportingTypeCommand : IRequest<ApiResponse<bool>>
    {
        
            public UpdateReportingTypeRequestDTO DTO { get; set; }

    public UpdateReportingTypeCommand(UpdateReportingTypeRequestDTO dTO)
    {
        this.DTO = dTO;
    }

}
     
}