using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    public class GetModuleHeadersCommand : IRequest<ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {

        public GetModuleChildInversRequestDTO DTO { get; set; }

        public GetModuleHeadersCommand(GetModuleChildInversRequestDTO dTO)
        {
            DTO = dTO;
        }

    }
}
