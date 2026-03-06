using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.EmployeeCmd.Queries
{
    public class GetAllEmployeeWithTypeQuery : IRequest<ApiResponse<List<GetEmployeeTypeResponseDTO>>>
    {
        public GetEmployeeTypeRequestDTO DTO { get; set; }

        public GetAllEmployeeWithTypeQuery(GetEmployeeTypeRequestDTO dTO)
        {
            this.DTO = dTO;

        }
    }
}
