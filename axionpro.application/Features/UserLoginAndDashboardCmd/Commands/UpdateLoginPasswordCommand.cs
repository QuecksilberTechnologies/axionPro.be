using axionpro.application.DTOs.UserLogin;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Commands
{
       public class UpdateLoginPasswordCommand : IRequest<ApiResponse<UpdateLoginPasswordResponseDTO>>
    {
        public LoginRequestDTO? setLoginPasswordRequest { get; set; }


        public UpdateLoginPasswordCommand(LoginRequestDTO? setLoginPasswordRequest)
        {
            this.setLoginPasswordRequest = setLoginPasswordRequest;
        }



    }
}
