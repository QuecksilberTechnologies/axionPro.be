using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Designation.Custom;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Interface for Designation repository.
    /// Provides CRUD and utility operations for multi-tenant designations.
    /// </summary>
    public interface IDesignationRepository
    {
        Task<ApiResponse<List<GetDesignationOptionResponseDTO?>>> GetOptionAsync(GetDesignationOptionRequestDTO dto , long tenantId);

        Task<int> AutoCreateDesignationAsync(List<Designation> designations, int departmentId);       
        Task<PagedResponseDTO<GetDesignationResponseDTO>> CreateAsync(CreateDesignationRequestDTO dto, long TenantId, long EmployeeId);     
        Task<bool> DeleteDesignationAsync(DeleteDesignationRequestDTO   designation, long EmployeeId);       
        Task<PagedResponseDTO<GetDesignationResponseDTO>> GetAsync(GetDesignationRequestDTO dTO , long tenantId , int Id);    
        Task<bool> UpdateDesignationAsync(UpdateDesignationRequestDTO designation, long EmployeeId); 
        Task<bool> CheckDuplicateValueAsync(long destrictedTenantId, string value);
        Task<GetSingleDesignationResponseDTO?> GetByIdAsync(GetSingleDesignationRequestDTO dto);




 


    }
}
