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
using axionpro.application.DTOS.Module.ParentModule;
namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    public class CreateParentModuleRequestCommand : IRequest<ApiResponse<List<GetParentModuleResponseDTO>>>
    {

        public CreateParentModuleRequestDTO DTO { get; set; }

        public CreateParentModuleRequestCommand(CreateParentModuleRequestDTO dTO)
        {
            DTO = dTO;
        }

    }
     
}
