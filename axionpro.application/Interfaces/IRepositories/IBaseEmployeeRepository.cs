

using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Contact;

using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.DTOS.Pagination;

using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Interfaces.IRepositories;

public interface IBaseEmployeeRepository
{

    #region Employee-Base-info

 
     
        Task<bool> UpdateSectionVerifyStatusAsync(
        string sectionName,     // 🔥 MISSING PARAM
        long employeeId,
        long tenantId,
        bool isVerified,
        bool isEditAllowed,
        long userId,
        CancellationToken ct);
    Task<GetLeaveBalanceToEmployeeResponseDTO> UpdateLeaveBalanceToEmployee(UpdateLeaveBalanceToEmployeeRequestDTO updateLeaveBalanceTo);  // Ensure this returns Task<Employee>   

     Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> GetInfo(GetBaseEmployeeRequestDTO dto);
      Task<PagedResponseDTO<GetAllEmployeeInfoResponseDTO>> GetAllInfo(GetAllEmployeeInfoRequestDTO dto);
      Task<Employee?> IsEmployeeExist(string EmployeeCode, long tenantId, bool track = true);

       public Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> CreateAsync(Employee entity);
       public Task<GetBaseEmployeeResponseDTO> CreateEmployeeAsync(Employee entity, LoginCredential loginCredential,UserRole userRole);
       public Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> AddImageAsync(EmployeeImage entity);
    
      public Task<GetMinimalEmployeeResponseDTO> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns 
      public Task<EmployeeImage> IsImageExist(long? Id, bool IsActive);  // Ensure this returns 
      public Task<bool>  UpdateProfileImage(EmployeeImage employeeImageInfo);
      public Task<Employee?> GetByIdAsync(long id, long tenantId, bool track = true);
      public Task<bool> UpdateEditStatus(long EmployeeId, long UserId, bool Status);
      public Task<bool> UpdateVerificationStatus(long EmployeeId, long UserId, bool Status);
      public Task<bool> DeleteAllAsync(Employee employee);
      public Task<bool> ActivateAllEmployeeAsync(Employee employee, bool IsActive);
      public  Task<long> AutoCreated(Employee entity);
     
      public Task<bool> UpdateEmployeeAsync(Employee entity, long tenantId);
      public Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> GetImage(GetEmployeeImageRequestDTO dto);
           public  Task<string?> ProfileImage(long employeeId);
      public Task<List<CompletionSectionDTO>> GetEmployeeCompletionAsync(long employeeId);

    #endregion




}

