using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

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
