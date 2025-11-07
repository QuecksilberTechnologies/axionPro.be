using axionpro.application.DTOs.Employee;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Command
{
    public class UpdateDependentCommand : IRequest<ApiResponse<bool>>
    {
        public GenricUpdateRequestDTO DTO { get; set; }

        public UpdateDependentCommand(GenricUpdateRequestDTO dto)
        {
            DTO = dto;
        }

    }
}