using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IDepartmentRepository
    {


            Task<GetSingleDepartmentResponseDTO?> GetByIdAsync(GetSingleDepartmentRequestDTO dto);
            Task<ApiResponse<List<GetDepartmentOptionResponse?>>> GetOptionAsync(GetOptionRequestDTO dto);
           
            Task<bool> DeleteAsync(DeleteDepartmentRequestDTO requestDTO, long EmployeeId, int Id); 
            Task<PagedResponseDTO<GetDepartmentResponseDTO>> CreateAsync(CreateDepartmentRequestDTO dto);
            Task<PagedResponseDTO<GetDepartmentResponseDTO>> GetAsync(GetDepartmentRequestDTO request);
           
            Task<int> AutoCreateDepartmentSeedAsync(List<Department>? departments);
           
           Task<bool> UpdateAsync(UpdateDepartmentRequestDTO requestDTO );
          
            Task<bool> ExistsAsync(long id, long tenantId);
           Task<Dictionary<string, int>> GetDepartmentNameIdMapAsync(long tenantId);

    }
}
