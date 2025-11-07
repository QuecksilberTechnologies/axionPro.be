 
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Commands
{
    public class LoginCommand : IRequest<ApiResponse<LoginResponseDTO>>
    {
        public LoginRequestDTO RequestLoginDTO { get; set; }


        public LoginCommand(LoginRequestDTO loginRequestDTO)
        {
            RequestLoginDTO = loginRequestDTO;
        }



    }



}
