using axionpro.application.DTOs.Registration;
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

    public class CandidateRegistrationCommand : IRequest<ApiResponse<CandidateResponseDTO>>
    {
        public CandidateRequestDTO RequestCandidateRegistrationDTO { get; set; }


        public CandidateRegistrationCommand(CandidateRequestDTO candidateRegistrationRequestDTO)
        {
            RequestCandidateRegistrationDTO = candidateRegistrationRequestDTO;
        }



    }
}
