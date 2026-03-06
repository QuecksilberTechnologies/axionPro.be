using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.EmployeeType;
using axionpro.domain.Entity;
namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeTypeRepository
    {
      public  Task<EmployeeType> GetEmployeeTypeByIdAsync(int? employeeTypeId);

       public Task<IEnumerable<GetEmployeeTypeResponseOptionDTO>> GetEmployeeTypesOptionAsync(GetOptionRequestDTO getPolicyTypeDTO);

    }
}
