using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class OperationRepository : IOperationRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<OperationRepository> _logger;

        public OperationRepository(
            WorkforceDbContext context,
            ILogger<OperationRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Operation>> CreateOperationAsync(Operation operation)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(operation);

                await _context.Operations.AddAsync(operation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Operation added successfully: {OperationName}", operation.OperationName);
                return await GetAllOperationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an operation.");
                throw;
            }
        }

        public async Task<bool> DeleteOperationAsync(Operation operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            var existingOperation = await _context.Operations
                .FirstOrDefaultAsync(item => item.Id == operation.Id);

            if (existingOperation == null)
            {
                return false;
            }

            existingOperation.IsActive = false;
            existingOperation.UpdatedById = operation.UpdatedById;
            existingOperation.UpdateDateTime = operation.UpdateDateTime;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Operation with ID {OperationId} was deactivated.", operation.Id);
            return true;
        }

        public async Task<List<Operation>> GetAllOperationAsync()
        {
            _logger.LogInformation("Fetching all operations from the database.");

            return await _context.Operations
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Operation?> GetOperationByIdAsync(int id)
        {
            return await _context.Operations
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        public async Task<List<Operation>> UpdateOperationAsync(Operation operation)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(operation);

                var existingOperation = await _context.Operations
                    .FirstOrDefaultAsync(item => item.Id == operation.Id);

                if (existingOperation == null)
                {
                    _logger.LogWarning("Operation with ID {OperationId} was not found.", operation.Id);
                    return new List<Operation>();
                }

                if (!string.IsNullOrWhiteSpace(operation.OperationName))
                {
                    existingOperation.OperationName = operation.OperationName;
                }

                if (operation.IconImage is not null)
                {
                    existingOperation.IconImage = operation.IconImage;
                }

                if (operation.Remark is not null)
                {
                    existingOperation.Remark = operation.Remark;
                }

                if (operation.UpdatedById is > 0)
                {
                    existingOperation.UpdatedById = operation.UpdatedById;
                }

                if (operation.OperationType is > 0)
                {
                    existingOperation.OperationType = operation.OperationType;
                }

                if (operation.UpdateDateTime is not null && operation.UpdateDateTime != default)
                {
                    existingOperation.UpdateDateTime = operation.UpdateDateTime;
                }

                existingOperation.IsActive = operation.IsActive;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Operation with ID {OperationId} updated successfully.", operation.Id);

                return await GetAllOperationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating operation with ID {OperationId}.", operation?.Id);
                throw;
            }
        }
    }
}
