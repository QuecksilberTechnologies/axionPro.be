using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Queries
{
    public class GetBaseEmpldoyeeInfoQuery : IRequest <ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        public GetBaseEmployeeRequestDTO DTO { get; }

        public GetBaseEmpldoyeeInfoQuery(GetBaseEmployeeRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
}
