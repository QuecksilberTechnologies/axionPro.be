using GetEmployeeTypeResponseDTO = axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO;
using axionpro.application.DTOS.Employee.Type;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeTypeRepository
{
    Task<EmployeeType?> GetEmployeeTypeByIdAsync(long tenantId, int employeeTypeId, CancellationToken cancellationToken);
    Task<EmployeeType?> GetForUpdateAsync(long tenantId, int employeeTypeId, CancellationToken cancellationToken);
    Task<List<GetEmployeeTypeResponseDTO>> GetAllAsync(long tenantId, CancellationToken cancellationToken);
    Task<bool> NameExistsAsync(long tenantId, string typeName, int? excludeEmployeeTypeId, CancellationToken cancellationToken);
    Task<bool> HasDeletionDependenciesAsync(long tenantId, int employeeTypeId, CancellationToken cancellationToken);
    Task<GetEmployeeTypeResponseDTO> CreateAsync(long tenantId, long actorId, CreateEmployeeTypeDTO request,
        CancellationToken cancellationToken);
    Task<GetEmployeeTypeResponseDTO> UpdateAsync(EmployeeType entity, CancellationToken cancellationToken);
    Task<bool> SoftDeleteAsync(EmployeeType entity, CancellationToken cancellationToken);
    Task<int> EnsureOnboardingTypeAsync(long tenantId, long actorId, CancellationToken cancellationToken);
}
