using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeePolicyEnrollmentRepository
{
    // 🔹 CREATE
    Task<EmployeePolicyEnrollment> AddAsync(EmployeePolicyEnrollment entity);

    // 🔹 GET BY ID
    Task<EmployeePolicyEnrollment?> GetByIdAsync(long id, long tenantId);

    // 🔹 GET BY EMPLOYEE
    Task<List<EmployeePolicyEnrollment>> GetByEmployeeIdAsync(long employeeId, long tenantId);

    // 🔹 UPDATE
    Task<bool> UpdateAsync(EmployeePolicyEnrollment entity);

    // 🔹 SOFT DELETE
    Task<bool> SoftDeleteAsync(EmployeePolicyEnrollment entity);
      Task<EmployeePolicyEnrollment?> GetExistingAsync(long employeeId, int policyTypeId, int insurancePolicyId, long tenantId);
         
}
 
}
