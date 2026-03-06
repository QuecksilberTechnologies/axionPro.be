using axionpro.application.DTOs.Leave;

 
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.WorkflowStage.Commands
{
    public class CreateWorkflowStageCommand : IRequest<ApiResponse<List<GetWorkflowStageResponseDTO>>>
    {
        
            public CreateWorkflowStageRequestDTO DTO { get; set; }

            public CreateWorkflowStageCommand(CreateWorkflowStageRequestDTO dTO)
            {
                this.DTO = dTO;
            }

        }
     
}
