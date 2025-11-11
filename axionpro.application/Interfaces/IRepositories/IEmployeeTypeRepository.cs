using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.EmployeeType;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.EmployeeType;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeTypeRepository
    {
      public  Task<EmployeeType> GetEmployeeTypeByIdAsync(int? employeeTypeId);

       public Task<IEnumerable<GetEmployeeTypeResponseOptionDTO>> GetEmployeeTypesOptionAsync(GetOptionRequestDTO getPolicyTypeDTO);

    }
}
