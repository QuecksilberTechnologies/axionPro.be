using axionpro.application.DTOs.Leave;
 
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.LeaveCmd.Commands
{
    public class CreateLeaveTypeCommand: IRequest<ApiResponse<List<GetLeaveTypResponseDTO>>>
    {
        
            public CreateLeaveTypeRequestDTO createLeaveTypeDTO { get; set; }

            public CreateLeaveTypeCommand(CreateLeaveTypeRequestDTO createLeaveTypeDTO)
            {
                this.createLeaveTypeDTO = createLeaveTypeDTO;
            }

        }
     
}
