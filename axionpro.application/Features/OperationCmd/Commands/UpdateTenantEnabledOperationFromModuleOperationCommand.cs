using axionpro.application.DTOs.Operation;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.OperationCmd.Commands
{
    public class UpdateTenantEnabledOperationFromModuleOperationCommand  : IRequest<ApiResponse<UpdateTenantEnabledOperationFromModuleOperationResponseDTO>>
    {

        public UpdateTenantEnabledOperationFromModuleOperationRequestDTO dto { get; set; }

        public UpdateTenantEnabledOperationFromModuleOperationCommand(UpdateTenantEnabledOperationFromModuleOperationRequestDTO dto)
        {
            this.dto = dto;
        }

    }
    
}
