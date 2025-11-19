using axionpro.application.DTOS.Employee.CompletionPercentage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeCompletionRepository
    {
        Task<EmployeeCompletionResponseDTO> GetEmployeeCompletionAsync(long employeeId);
    }
}
