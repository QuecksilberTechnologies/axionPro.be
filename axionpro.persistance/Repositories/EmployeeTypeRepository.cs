using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.EmployeeType;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class EmployeeTypeRepository : IEmployeeTypeRepository
    {
       
        private ILogger _logger;
        private WorkforceDbContext _context;
        private ILogger<EmployeeTypeRepository> logger;

      

        public EmployeeTypeRepository(WorkforceDbContext context, ILogger<EmployeeTypeRepository> logger)
        {
            this._context = context;
            this.logger = logger;
        }

        public async Task<IEnumerable<GetEmployeeTypeResponseDTO>> GetAllEmployeeTypesAsync(GetEmployeeTypeRequestDTO request)
        {
            try
            {
                // 🔹 Base query
                var query = _context.EmployeeTypes.AsQueryable();

                // 🔹 Filter: Active + Not SoftDeleted
                query = query.Where(et =>
                    et.IsActive == request.IsActive
                    && (et.IsSoftDeleted == null || et.IsSoftDeleted == false)
                );

                // 🔹 Data map into ResponseDTO
                var list = await query
                    .Select(et => new GetEmployeeTypeResponseDTO
                    {
                        Id = et.Id,
                        TypeName = et.TypeName ?? string.Empty,
                        Description = et.Description ?? string.Empty,
                        Remark = et.Remark ?? string.Empty,
                        IsActive = et.IsActive
                    })
                    .ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                // 🔹 Agar error aaya toh empty list return karo aur log karo
                _logger.LogError(ex, "Error while fetching EmployeeTypes");
                return Enumerable.Empty<GetEmployeeTypeResponseDTO>();
            }
        }

   





        // Method to fetch employee type by ID
        public async Task<EmployeeType> GetEmployeeTypeByIdAsync(int? employeeTypeId)
        {
            try
            {
                _logger?.LogInformation("Fetching employee type for ID: {EmployeeTypeId}", employeeTypeId);

                //var employeeType = nullawait _context.EmployeeTypes
                //                         .Include(et => et.Role)          // Include Role entity
                //                         .Include(et => et.EmployeeTypeBasicMenus)    // Include CommonMenus collection
                //                         .FirstOrDefaultAsync(et => et.Id == employeeTypeId);

                //if (employeeType == null)
                //{
                //    _logger?.LogWarning("Employee type not found with ID: {EmployeeTypeId}", employeeTypeId);
                //    return null;
                //}

              //  return employeeType;
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while fetching employee type for ID: {EmployeeTypeId}", employeeTypeId);
                throw;
          
            }
        }

        Task<EmployeeType> IEmployeeTypeRepository.GetEmployeeTypeByIdAsync(int? employeeTypeId)
        {
            throw new NotImplementedException();
        }
    }
}
