using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.EnrolledPolicy;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeDependentInsuranceMappingRepository
    {
        // 🔹 BULK INSERT (🔥 MOST IMPORTANT)
        Task<bool> AddRangeAsync(List<EmployeePolicyDependentMapping> entities);

        // 🔹 GET BY ENROLLMENT
        Task<List<EmployeePolicyDependentMapping>> GetByEnrollmentIdAsync(long enrollmentId, long tenantId);
        Task<List<GetEmployeeEnrolledResponseDTO>> GetByEmployeeIdAsync(long employeeId, long tenantId);

        // 🔹 DELETE (REPLACE SCENARIO)
        Task<bool> SoftDeleteByEnrollmentIdAsync(List<EmployeePolicyDependentMapping> entities);
    }
}
