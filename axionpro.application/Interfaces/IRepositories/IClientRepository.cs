using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
     public interface IClientRepository
    {
        // Create a new role
        Task<List<ClientType>> CreateClientTypeAsync(ClientType leaveType);

        // Get a role by its Id
        Task<ClientType> GetClientTypeByIdAsync(int Id);

        // Get all roles
        Task<List<ClientType>> GetAllClientTypeAsync();

        // Update an existing role
        Task<List<ClientType>> UpdateClientTypeAsync(ClientType leaveType);

        // Delete a role by its Id
        Task<bool> DeleteClientTypeAsync(int Id);
    }
}
