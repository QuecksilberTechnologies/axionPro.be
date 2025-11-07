using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace axionpro.application.Features.ModuleCmd.Common.Commands
{
    public class CreateCommonModuleCommand : IRequest<ApiResponse<List<GetCommonModuleResponseDTO>>>
    {

        public CreateCommonModuleRequestDTO DTO { get; set; }

        public CreateCommonModuleCommand(CreateCommonModuleRequestDTO dTO)
        {
            DTO = dTO;
        }

    }
     
}
