using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Registration;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.RegistrationCmd.Commands
{
    public class CreateTenantCommand : IRequest<ApiResponse<TenantCreateResponseDTO>>
    {
        public TenantRequestDTO TenantCreateRequestDTO { get; set; }

        public CreateTenantCommand(TenantRequestDTO createRequestDTO)
        {
            TenantCreateRequestDTO = createRequestDTO;
        }

    }
}
