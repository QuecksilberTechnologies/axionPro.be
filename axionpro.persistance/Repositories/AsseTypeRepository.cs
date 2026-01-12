using AutoMapper;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{

    public class AssetTypeRepository : IAssetTypeRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetTypeRepository> _logger;

        public AssetTypeRepository(
            WorkforceDbContext context,
            ILogger<AssetTypeRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }
        public async Task<List<GetTypeResponseDTO>> AddAsync(AddTypeRequestDTO? dto)
        {
            try
            {
                              

                if (string.IsNullOrWhiteSpace(dto.TypeName))
                {
                    _logger.LogWarning("Asset Type name is missing for TenantId: {TenantId}", dto.Prop.TenantId);
                    throw new ArgumentException("TypeName is required.");
                }
                _logger.LogInformation("Adding new Asset Type '{TypeName}' for TenantId: {TenantId}", dto.TypeName, dto.Prop.TenantId);

                // ✅ Step 2: Duplicate check
                var existingType = await _context.AssetTypes
                    .FirstOrDefaultAsync(t =>
                        t.TenantId == dto.Prop.TenantId &&
                        t.TypeName.ToLower() == dto.TypeName.ToLower() &&
                        (t.IsSoftDeleted == null || t.IsSoftDeleted == false));

                if (existingType != null)
                {
                    _logger.LogWarning("Duplicate Asset Type detected for TenantId: {TenantId}, TypeName: {TypeName}",
                        dto.Prop.TenantId, dto.TypeName);

                    throw new InvalidOperationException($"Asset Type '{dto.TypeName}' already exists for this tenant.");
                }
                // ✅ Entity creation (DB ONLY)
                var entity = new AssetType
                {
                    TenantId = dto.Prop.TenantId,
                    AssetCategoryId = dto.AssetCategoryId,
                    TypeName = dto.TypeName,
                    Description = dto.Description?.Trim(),
                    IsActive = dto.IsActive,
                    IsSoftDeleted = false,
                    AddedById = dto.Prop.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };


               
                 

                // ✅ Step 4: Insert into DB
                await _context.AssetTypes.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Asset Type added successfully with Id: {Id}", entity.Id);

                // ✅ Step 5: Return all active types (for the same Tenant)
                var allTypes = await _context.AssetTypes
                    .Where(x => x.TenantId == dto.Prop.TenantId && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                return _mapper.Map<List<GetTypeResponseDTO>>(allTypes);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Null DTO received while adding Asset Type.");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error in AddAsync for Asset Type.");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Duplicate Asset Type detected while adding.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding Asset Type for TenantId: {TenantId}", dto?.Prop.TenantId);
                throw new Exception("An unexpected error occurred while adding Asset Type.", ex);
            }
        }


        public async Task<bool> DeleteAsync(DeleteTypeRequestDTO dto)
        {
            try
            {
                // ✅ Step 1: Validation
                if (dto == null)
                {
                    _logger.LogWarning("DeleteAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Request cannot be null.");
                }

                if (dto.Id <= 0)
                {
                    _logger.LogWarning("Invalid Asset Type Id provided for deletion: {Id}", dto.Id);
                    throw new ArgumentException("Valid Asset Type Id is required for deletion.");
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Step 2: Fetch record from DB
                var existingType = await context.AssetTypes
                    .FirstOrDefaultAsync(x => x.Id == dto.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false));

                if (existingType == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing Asset Type with Id: {Id}", dto.Id);
                    throw new KeyNotFoundException($"Asset Type with Id {dto.Id} not found.");
                }

                _logger.LogInformation("Deleting Asset Type with Id: {Id}, Name: {Name}", existingType.Id, existingType.TypeName);

                // ✅ Step 3: Soft delete (mark as deleted instead of removing)
                existingType.IsSoftDeleted = true;
                existingType.IsActive = false;
                existingType.SoftDeletedById = dto.Prop.EmployeeId;
                existingType.DeletedDateTime = DateTime.UtcNow;

                // ✅ Step 4: Save changes
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Asset Type with Id: {Id} deleted successfully.", dto.Id);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Null DTO passed to DeleteAsync.");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed in DeleteAsync for Asset Type.");
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Asset Type not found while attempting delete.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting Asset Type with Id: {Id}", dto?.Id);
                throw new Exception("An unexpected error occurred while deleting Asset Type.", ex);
            }
        }

        public async Task<List<GetTypeResponseDTO>> GetAllAsync(GetTypeRequestDTO? dto)
        {
            try
            {
               
                _logger.LogInformation("Fetching Asset Types with applied filters...");

                // ✅ Include related AssetCategory
                IQueryable<AssetType> query = _context.AssetTypes
                    .Include(x => x.AssetCategory)
                    .Where(x => x.IsSoftDeleted != true && x.TenantId == dto.Prop.TenantId);

                // 🔹 Apply Filters
                if (dto?.TypeId is > 0)
                    query = query.Where(x => x.Id == dto.TypeId.Value);
                else if (dto?.CategoryId is > 0)
                    query = query.Where(x => x.AssetCategoryId == dto.CategoryId.Value);

                if (dto?.IsActive.HasValue == true)
                    query = query.Where(x => x.IsActive == dto.IsActive.Value);

                // 🔹 Sort by AddedDateTime DESC
                query = query.OrderByDescending(x => x.AddedDateTime);

                // 🔹 Project to DTO directly (better than mapping after ToList)
                var result = await query
                    .Select(x => new GetTypeResponseDTO
                    {
                        Id = x.Id,
                        TenantId = x.TenantId,
                        AssetCategoryId = x.AssetCategoryId,
                        CategoryName = x.AssetCategory != null ? x.AssetCategory.CategoryName : null, // ✅ fixed
                        TypeName = x.TypeName,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        AddedById = x.AddedById,
                        AddedDateTime = x.AddedDateTime,
                        UpdatedById = x.UpdatedById,
                        UpdatedDateTime = x.UpdatedDateTime
                    })
                    .ToListAsync();

                if (!result.Any())
                {
                    _logger.LogWarning("No Asset Types found for given filters (TenantId: {TenantId}, TypeId: {TypeId}, CategoryId: {CategoryId}).",
                        dto?.Prop.TenantId, dto?.TypeId, dto?.CategoryId);
                    return new List<GetTypeResponseDTO>();
                }

                _logger.LogInformation("Fetched {Count} Asset Types for TenantId: {TenantId}.", result.Count, dto?.Prop.TenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Asset Types for TenantId: {TenantId}.", dto?.Prop.TenantId);
                throw new Exception("An unexpected error occurred while fetching Asset Types.", ex);
            }
        }

        public async Task<bool> UpdateAsync(UpdateTypeRequestDTO? dto)
        {
           // await using var _context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // Step 1️⃣: Validate input
                if (dto == null)
                {
                    _logger.LogWarning("UpdateAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Update request cannot be null.");
                }

                if (dto.Id <= 0)
                {
                    _logger.LogWarning("Invalid Type Id ({Id}) provided in UpdateAsync.", dto.Id);
                    throw new ArgumentException("Type Id must be greater than zero.");
                }

                _logger.LogInformation("Attempting to update Asset Type with Id: {Id}", dto.Id);

                // Step 2️⃣: Fetch the existing entity
                var existingType = await _context.AssetTypes
                    .FirstOrDefaultAsync(x => x.Id == dto.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false));

                if (existingType == null)
                {
                    _logger.LogWarning("Asset Type with Id {Id} not found or is deleted.", dto.Id);
                    throw new KeyNotFoundException($"Asset Type with Id {dto.Id} not found.");
                }

                // Step 3️⃣: Conditional field updates
                existingType.AssetCategoryId = dto.CategoryId > 0 ? dto.CategoryId : existingType.AssetCategoryId;
                existingType.TypeName = !string.IsNullOrWhiteSpace(dto.TypeName) ? dto.TypeName : existingType.TypeName;
                existingType.Description = !string.IsNullOrWhiteSpace(dto.Description) ? dto.Description : existingType.Description;

                if (dto.IsActive.HasValue)
                {
                    existingType.IsActive = dto.IsActive.Value;
                }

                existingType.UpdatedById = dto.Prop.EmployeeId;
                existingType.UpdatedDateTime = DateTime.UtcNow;

                // Step 4️⃣: Save changes within transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                _context.AssetTypes.Update(existingType);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _logger.LogInformation("Asset Type with Id {Id} updated successfully.", dto.Id);
                    return true;
                }

                _logger.LogWarning("No rows affected during Asset Type update for Id {Id}.", dto.Id);
                return false;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null DTO passed to UpdateAsync.");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid Id or arguments provided for Asset Type update.");
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Asset Type not found for Id: {Id}", dto?.Id);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Validation failed for Asset Type update. {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating Asset Type with Id: {Id}", dto?.Id);
                throw new Exception("An unexpected error occurred while updating Asset Type.", ex);
            }
        }


    }
}
