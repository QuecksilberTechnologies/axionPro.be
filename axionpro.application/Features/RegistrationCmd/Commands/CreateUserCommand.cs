using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.RegistrationCmd.Commands
{
    public class CreateUserCommand  : IRequest<ApiResponse<LoginResponseDTO>>
    {
        public LoginRequestDTO LoginRequestDTO { get; set; }

        public CreateUserCommand(LoginRequestDTO loginRequestDTO)
        {
            LoginRequestDTO = loginRequestDTO;
        }

    }
    
}
