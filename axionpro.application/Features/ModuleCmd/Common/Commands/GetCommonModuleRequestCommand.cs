using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;


using axionpro.domain.Entity;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOS.Module.CommonModule;
namespace axionpro.application.Features.ModuleCmd.Common.Commands
{
    public class GetCommonModuleRequestCommand : IRequest<ApiResponse<List<GetCommonModuleResponseDTO>>>
    {

        public GetCommonModuleRequestDTO DTO { get; set; }

        public GetCommonModuleRequestCommand(GetCommonModuleRequestDTO dTO)
        {
            DTO = dTO;
        }

    }
     
}
