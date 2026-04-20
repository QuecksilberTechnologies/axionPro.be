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
    }
}
