using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IOperationRepository
    {
        Task<List<Operation>> CreateOperationAsync(Operation operation);

        Task<Operation?> GetOperationByIdAsync(int id);

        Task<List<Operation>> GetAllOperationAsync();

        Task<List<Operation>> UpdateOperationAsync(Operation operation);

        Task<bool> DeleteOperationAsync(Operation operation);
    }
}
