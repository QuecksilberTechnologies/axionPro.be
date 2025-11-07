using AutoMapper;
using axionpro.application.Constants;
 
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Interfaces.IRepositories;
 
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace axionpro.persistance.Repositories
{
    /// <summary>
    /// Repository for managing Workflow Stages.
    /// Implements IWorkflowStagesRepository with full CRUD operations.
    /// </summary>
    public class WorkflowStageRepository : IWorkflowStagesRepository
    {
        #region Fields
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly WorkforceDbContext _context;
        private readonly ILogger<WorkflowStageRepository> _logger;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public WorkflowStageRepository(
            WorkforceDbContext context,
            ILogger<WorkflowStageRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }
        #endregion

        #region CRUD Methods

        /// <summary>
        /// Fetch all active workflow stages.
        /// </summary>
        public async Task<List<GetWorkflowStageResponseDTO>> AllAsync(GetWorkflowStageRequestDTO dto)
        {
            try
            {
                await using var contextFac = await _contextFactory.CreateDbContextAsync();

                var query = contextFac.WorkflowStages.AsQueryable();

                query = query.Where(s => (s.IsSoftDeleted == null || s.IsSoftDeleted == false) && s.StageOrder == 1);

                if (dto.IsActive != null)
                    query = query.Where(s => s.IsActive == dto.IsActive);

                var stages = await query.ToListAsync();

                return _mapper.Map<List<GetWorkflowStageResponseDTO>>(stages);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;

                if (errorMessage.Contains("Invalid column name"))
                {
                    _logger.LogError(ex, $"⚠️ DB column mismatch detected: {errorMessage}");
                    return new List<GetWorkflowStageResponseDTO>();
                }

                _logger.LogError(ex, "❌ Unexpected error fetching workflow stages.");
                return new List<GetWorkflowStageResponseDTO>();
            }
        }


        /// <summary>
        /// Fetch a workflow stage by its unique Id.
        /// </summary>
        public async Task<GetWorkflowStageResponseDTO?> GetByIdAsync(long id)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var stage = await context.WorkflowStages
                    .FirstOrDefaultAsync(s => s.Id == id && s.IsActive && (s.IsSoftDeleted == null || s.IsSoftDeleted == false));

                if (stage == null)
                {
                    _logger.LogWarning("⚠️ WorkflowStage with Id {Id} not found.", id);
                    return null;
                }

                return _mapper.Map<GetWorkflowStageResponseDTO>(stage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching workflow stage by Id {Id}.", id);
                return null;
            }
        }

        /// <summary>
        /// Add a new workflow stage.
        /// </summary>
        public async Task<List<GetWorkflowStageResponseDTO>> AddAsync(CreateWorkflowStageRequestDTO dto)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var entity = _mapper.Map<WorkflowStage>(dto);
                entity.IsActive = ConstantValues.IsByDefaultTrue;
                entity.AddedById = dto.EmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
              

                await context.WorkflowStages.AddAsync(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ WorkflowStage added successfully with Id {Id}.", entity.Id);

                // Return all active workflow stages after addition
                var stages = await context.WorkflowStages
                    .Where(s => s.IsActive && (s.IsSoftDeleted == null || s.IsSoftDeleted == false))
                    .ToListAsync();

                return _mapper.Map<List<GetWorkflowStageResponseDTO>>(stages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error adding new workflow stage.");
                return new List<GetWorkflowStageResponseDTO>();
            }
        }

        /// <summary>
        /// Update an existing WorkflowStage record in the database.
        /// </summary>
        /// <param name="dto">The DTO containing updated data for WorkflowStage.</param>
        /// <returns>Returns true if update succeeds; otherwise false.</returns>
        public async Task<bool> UpdateAsync(UpdateWorkflowStageRequestDTO dto)
        {
            try
            {
                // 🧠 Step 1: Validate input
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid WorkflowStage update request received.");
                    return false;
                }

                // 🧠 Step 2: Create a new DbContext instance from factory
                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🧠 Step 3: Fetch the entity by Id (only active and not soft-deleted)
                var entity = await context.WorkflowStages
                    .FirstOrDefaultAsync(s => s.Id == dto.Id && (s.IsSoftDeleted == null || s.IsSoftDeleted == false));

                if (entity == null)
                {
                    _logger.LogWarning("⚠️ WorkflowStage with Id {Id} not found or inactive/soft-deleted.", dto.Id);
                    return false;
                }

                // 🧠 Step 4: Handle nullable IsActive — preserve old value if dto.IsActive is null
                if (dto.IsActive == null)
                    dto.IsActive = entity.IsActive;

                // 🧠 Step 5: Map updated values
                _mapper.Map(dto, entity);

                // 🧠 Step 6: Update audit fields
                entity.UpdatedById = dto.EmployeeId;
                entity.UpdatedDateTime = DateTime.UtcNow;
               
                // 🧠 Step 7: Save changes
                context.WorkflowStages.Update(entity);
                var result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    _logger.LogInformation("✅ WorkflowStage with Id {Id} updated successfully.", dto.Id);
                    return true;
                }

                _logger.LogWarning("⚠️ WorkflowStage update for Id {Id} did not modify any record.", dto.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating WorkflowStage with Id {Id}.", dto.Id);
                return false;
            }
        }

        /// <summary>
        /// Soft delete a workflow stage by Id.
        /// </summary>
        public async Task<bool> DeleteAsync(long id, long employeeId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var entity = await context.WorkflowStages
                    .FirstOrDefaultAsync(s => s.Id == id && (s.IsSoftDeleted == null || s.IsSoftDeleted == false));

                if (entity == null)
                {
                    _logger.LogWarning("⚠️ WorkflowStage with Id {Id} not found for delete.", id);
                    return false;
                }

                entity.IsActive = false;
                entity.IsSoftDeleted = true;
                entity.UpdatedById = employeeId;
                entity.UpdatedDateTime = DateTime.UtcNow;

                context.WorkflowStages.Update(entity);
                var result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    _logger.LogInformation("🗑️ WorkflowStage with Id {Id} soft deleted successfully.", id);
                    return true;
                }

                _logger.LogWarning("⚠️ No changes made while deleting WorkflowStage with Id {Id}.", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deleting WorkflowStage with Id {Id}.", id);
                return false;
            }
        }

        #endregion
    }


}
