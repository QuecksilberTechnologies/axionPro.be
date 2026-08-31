using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeManagerMappingRepository
    {
          Task<bool> AddAsync(EmployeeManagerMapping entity);
          Task<bool>  ExistsPrimaryAsync(long employeeId, long tenantId);

          /// <summary>
          /// Checks whether the specified manager currently has an active direct-report mapping
          /// to the requested employee in the same tenant.
          /// </summary>
          Task<bool> IsCurrentDirectReportAsync(
              long tenantId,
              long managerId,
              long employeeId,
              CancellationToken cancellationToken = default);
    }
}
