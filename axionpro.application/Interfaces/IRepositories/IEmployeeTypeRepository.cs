using GetEmployeeTypeResponseDTO = axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO;
using axionpro.application.DTOS.Employee.Type;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeTypeRepository
{
    Task<EmployeeType?> GetEmployeeTypeByIdAsync(long tenantId, int employeeTypeId, CancellationToken cancellationToken);
    Task<List<GetEmployeeTypeResponseDTO>> GetAllAsync(long tenantId, CancellationToken cancellationToken);
    Task<GetEmployeeTypeResponseDTO> CreateAsync(long tenantId, long actorId, CreateEmployeeTypeDTO request,
        CancellationToken cancellationToken);
    Task<int> EnsureOnboardingTypeAsync(long tenantId, long actorId, CancellationToken cancellationToken);
}
