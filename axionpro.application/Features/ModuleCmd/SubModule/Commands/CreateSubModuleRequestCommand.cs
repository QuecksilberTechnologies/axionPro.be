using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Features.ModuleCmd.Parent.Handlers;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
 
namespace axionpro.application.Features.ModuleCmd.SubModule.Commands
{
    public class CreateSubModuleRequestCommand : IRequest<ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {

        public CreateSubModuleRequestDTO DTO { get; set; }

        public CreateSubModuleRequestCommand(CreateSubModuleRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
     
}
