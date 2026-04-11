using axionpro.application.DTOS.Device.Enroll;
using axionpro.application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeEnrollmentAtDeviceRepository
    {
        Task<bool> EnrollEmployeeAsync(RegisterEmployeeDTORequest dto);

        Task<bool> DeleteEmployeeAsync(DeleteEmployeeDTORequest dto);
    }
}
