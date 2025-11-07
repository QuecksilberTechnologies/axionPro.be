using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IOperationRepository
    {
        Task<List<Operation>> CreateOperationAsync(Operation operation);

        // Get a role by its Id
        Task<Operation> GetOperationByIdAsync(int Id);

        // Get all roles
        Task<List<Operation>> GetAllOperationAsync();

        // Update an existing role
        Task<List<Operation>> UpdateOperationAsync(Operation operation);

        // Delete a role by its Id
        Task<bool> DeleteOperationAsync(int Id);
    }
}
