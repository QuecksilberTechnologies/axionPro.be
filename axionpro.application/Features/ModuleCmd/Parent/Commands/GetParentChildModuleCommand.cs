using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    public class GetParentChildModuleCommand : IRequest<ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {

        public GetModuleChildInversRequestDTO DTO { get; set; }

        public GetParentChildModuleCommand(GetModuleChildInversRequestDTO dTO)
        {
            DTO = dTO;
        }

    }
}
