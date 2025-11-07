using axionpro.application.DTOs.PolicyType;
using axionpro.application.Wrappers;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Features.PolicyTypeCmd.Commands
{
    public class GetPolicyTypeCommand : IRequest<ApiResponse<List<GetPolicyTypeResponseDTO>>>
    {
        public CreatePolicyTypeRequestDTO DTO { get; set; }

        public GetPolicyTypeCommand(CreatePolicyTypeRequestDTO dto)
        {
            this.DTO = dto;
        }
    }
}
