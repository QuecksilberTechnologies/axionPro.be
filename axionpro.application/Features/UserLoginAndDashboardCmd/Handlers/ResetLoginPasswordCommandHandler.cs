using axionpro.application.DTOs.UserLogin;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{
    public class ResetLoginPasswordCommand : IRequest<ApiResponse<UpdatePasswordResponseDTO>>
    {
        //till completed
        public ResetLoginPasswordRequestDTO dto { get; set; }
        public ResetLoginPasswordCommand(ResetLoginPasswordRequestDTO dto)
        {
            this.dto = dto;
        }
        public class ResetLoginPasswordCommandHandler
        { }
    }
}
