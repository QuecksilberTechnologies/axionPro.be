using axionpro.application.DTOs.Transport;
using axionpro.application.DTOs.Verify;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

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
