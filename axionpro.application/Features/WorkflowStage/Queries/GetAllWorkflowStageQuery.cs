using axionpro.application.DTOs.Leave;
 
 
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.WorkflowStage.Queries
{
    public class GetAllWorkflowStageQuery : IRequest<ApiResponse<List<GetWorkflowStageResponseDTO>>>
    {
        public GetWorkflowStageRequestDTO DTO { get; set; }

    public GetAllWorkflowStageQuery(GetWorkflowStageRequestDTO dTO)
    {
        this.DTO = dTO;
    }
}
}
