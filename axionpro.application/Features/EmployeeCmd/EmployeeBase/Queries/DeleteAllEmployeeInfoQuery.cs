using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Queries
{
 
    public class DeleteEmployeeQuery : IRequest<ApiResponse<bool>>
    {
        public long Id { get; set; }

        public DeleteEmployeeQuery(long id)
        {
            Id = id;
        }
    }
}
