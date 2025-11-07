using axionpro.application.DTOs.Transport;
using axionpro.application.DTOs.Verify;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.VerifyEmailCmd.Commands
{
    public class VerifyEmailCommand : IRequest<ApiResponse<VerifyEmailResponseDTO>>
    {
        public VerifyEmailRequestDTO verifyEmailRequestDTO { get; set; }

        public VerifyEmailCommand(VerifyEmailRequestDTO verifyEmailRequestDTO)
        {
            this.verifyEmailRequestDTO = verifyEmailRequestDTO;
        }

    }
}
