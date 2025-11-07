using axionpro.application.DTOs.EmployeeType;
using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeTypeCmd.Queries
{
    public class GetAllEmployeeTypeQuery : IRequest<ApiResponse<List<GetEmployeeTypeResponseDTO>>>
    {
        public GetEmployeeTypeRequestDTO DTO { get; set; }

        public GetAllEmployeeTypeQuery(GetEmployeeTypeRequestDTO dTO)
        {
            this.DTO = dTO;
        }
    }
}
