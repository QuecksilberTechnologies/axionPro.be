using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeLeaveRepository
    {
        #region EmployeeLeaveAssign-info
        Task<List<GetEmployeeLeavePolicyMappingReponseDTO>> CreateEmployeeLeaveMapAsync(CreateEmployeeLeavePolicyMappingRequestDTO requestDTO);  // Ensure this returns Task<Employee>

        Task<bool> UpdateEmployeeLeaveMap(UpdateEmployeeLeavePolicyMappingRequestDTO updateEmployeeLeavePolicy);  // Ensure this returns Task<Employee>
        Task<bool> UpdateIsLeaveBalanceAssigned(long LeaveTypeMappingId);  // Ensure this returns Task<Employee>
        Task<long> DeleteEmployeeLeaveMap(CreateEmployeeLeavePolicyMappingRequestDTO employeeLeavePolicyMappingRequestDTO);  // Ensure this returns Task<Employee>
        Task<List<GetEmployeeLeavePolicyMappingReponseDTO>> GetAllEmployeeLeaveMap(GetEmployeeLeavePolicyMappingRequestDTO employeeLeavePolicyMappingRequestDTO);  // Ensure this returns Task<Employee>

        Task<GetEmployeeLeavePolicyMappingReponseDTO> AddLeaveBalanceToEmployee(AddLeaveBalanceToEmployeeRequestDTO addLeaveBalanceTo);  // Ensure this returns Task<Employee>   
        Task<GetLeaveBalanceToEmployeeResponseDTO> UpdateLeaveBalanceToEmployee(UpdateLeaveBalanceToEmployeeRequestDTO updateLeaveBalanceTo);  // Ensure this returns Task<Employee>   


        #endregion

    }
}
