using AutoMapper;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Commands
{
    public class EmployeeTypeBasicMenuCommand : IRequest<ApiResponse<AccessDetailResponseDTO>>
    {
       
        public AccessDetailRequestDTO AccessDetailDTO { get; set; }
        public EmployeeTypeBasicMenuCommand(AccessDetailRequestDTO accessRequestDTO)
        {
            AccessDetailDTO = accessRequestDTO;
        }

    }
}
