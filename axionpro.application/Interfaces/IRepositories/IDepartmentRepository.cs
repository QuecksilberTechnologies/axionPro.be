using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IDepartmentRepository
    {
        Task<GetSingleDepartmentResponseDTO?> GetByIdAsync(
            GetSingleDepartmentRequestDTO dto,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<List<GetDepartmentOptionResponse?>>> GetOptionAsync(
            GetOptionRequestDTO dto,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            DeleteDepartmentRequestDTO requestDTO,
            long employeeId,
            int id,
            CancellationToken cancellationToken = default);

        Task<PagedResponseDTO<GetDepartmentResponseDTO>> CreateAsync(
            CreateDepartmentRequestDTO dto,
            CancellationToken cancellationToken = default);

        Task<PagedResponseDTO<GetDepartmentResponseDTO>> GetAsync(
            GetDepartmentRequestDTO request,
            CancellationToken cancellationToken = default);

        Task<bool> AutoCreateDepartmentSeedAsync(List<Department>? departments, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UpdateDepartmentRequestDTO requestDTO,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            long id,
            long tenantId,
            CancellationToken cancellationToken = default);

        Task<Dictionary<string, int>> GetDepartmentNameIdMapAsync(
            long tenantId,
            CancellationToken cancellationToken = default);
    }
}