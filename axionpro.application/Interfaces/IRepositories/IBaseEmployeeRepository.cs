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
using System.Collections.Generic;

namespace axionpro.application.Interfaces.IRepositories;

public interface IBaseEmployeeRepository
{
    #region Employee-Base-info

    Task<bool> UpdateSectionVerifyStatusAsync(
        int sectionName,
        long employeeId,
        long tenantId,
        bool isVerified,
        bool isEditAllowed,
        long userId,
        CancellationToken ct);

    Task<bool> UpdateVerificationStatusByTabAsync(
        int tabInfoType,
        long employeeId,
        long userEmployeeId,
        bool isVerified,
        CancellationToken ct);

    Task<bool> UpdateEditableStatusByEntityAsync(
        int tabInfoType,
        long employeeId,
        long userEmployeeId,
        bool isVerified,
        CancellationToken ct);

    Task<GetLeaveBalanceToEmployeeResponseDTO> UpdateLeaveBalanceToEmployee(
        UpdateLeaveBalanceToEmployeeRequestDTO updateLeaveBalanceTo);

    Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> GetInfo(GetBaseEmployeeRequestDTO dto);

    Task<PagedResponseDTO<GetAllEmployeeInfoResponseDTO>> GetAllInfo(GetAllEmployeeInfoRequestDTO dto);

    Task<Employee?> IsEmployeeExist(string employeeCode, long tenantId, bool track = true);

    Task<GetBaseEmployeeResponseDTO> CreateEmployeeAsync(
        Employee entity,
        LoginCredential loginCredential,
        UserRole userRole);

    Task<GetEmployeeImageReponseDTO> AddImageAsync(EmployeeImage entity);

    Task<GetMinimalEmployeeResponseDTO> GetSingleRecordAsync(long id, bool isActive);

    Task<SummaryEmployeeInfo?> BuildEmployeeSummaryAsync(long employeeId, bool isActive);

    Task<EmployeeProfileSummaryInfo?> EmployeeProfileSummaryAsync(long employeeId, bool isActive);

    Task<EmployeeImage> IsImageExist(long id, bool isActive);

    Task<bool> UpdateProfileImage(EmployeeImage employeeImageInfo);

    Task<Employee?> GetByIdAsync(long id, long tenantId, bool track = true);

    Task<bool> DeleteAllAsync(Employee employee);

    Task<bool> ActivateAllEmployeeAsync(Employee employee, bool isActive);

    Task<bool> UpdateEmployeeAsync(Employee entity, long tenantId);

    Task<GetEmployeeImageReponseDTO> GetImage(GetEmployeeImageRequestDTO dto);

    Task<string?> ProfileImage(long employeeId);

    Task<List<CompletionSectionDTO>> GetEmployeeCompletionAsync(long employeeId);

    Task<GetBaseEmployeeResponseDTO?> GetCreatedEmployeeResponseAsync(
        long employeeId,
        CancellationToken cancellationToken = default);

    Task AddEmployeeAggregateAsync(
        Employee employee,
        LoginCredential loginCredential,
        UserRole userRole,
        CancellationToken cancellationToken = default);

    #endregion
}