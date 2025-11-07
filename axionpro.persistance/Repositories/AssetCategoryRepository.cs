using AutoMapper;
using axionpro.application.Constants;
 

using axionpro.application.Interfaces.IFileStorage;

using axionpro.application.Interfaces.IQRService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.AssetDTO.category;

namespace axionpro.persistance.Repositories
{
    public class AssetCategoryRepository : IAssetCategoryRepository
{
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetCategoryRepository> _logger;

        public AssetCategoryRepository(
            WorkforceDbContext context,
            ILogger<AssetCategoryRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }

        public async Task<bool> IsAssetCategoryDuplicate(AssetCategory asset)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                return await context.AssetCategories.AnyAsync(a =>
                    (a.CategoryName == asset.CategoryName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for duplicate asset with ID {AssetId}.", asset.Id);
                throw;  // Exception को रिथ्रो कर दें ताकि calling code को पता चल सके
            }
        }
        /// <summary>
        /// Retrieves all Asset Category records for a specific tenant with optional filters.
        /// </summary>
        /// <param name="dto">The filter criteria for retrieving asset categories.</param>
        /// <returns>A list of <see cref="GetCategoryResponseDTO"/> objects.</returns>
        public async Task<List<GetCategoryResponseDTO>> GetAllAsync(GetCategoryReqestDTO? dto)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ 1️⃣ Basic Validation Checks
                if (dto == null)
                {
                    _logger.LogWarning("GetAllAsync called with null DTO.");
                    return new List<GetCategoryResponseDTO>();
                }

                if (dto.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId provided: {TenantId}", dto.TenantId);
                    return new List<GetCategoryResponseDTO>();
                }

                // ✅ 2️⃣ Build Query with Filters
                IQueryable<AssetCategory> query = context.AssetCategories
                    .Where(x => x.TenantId == dto.TenantId && x.IsSoftDeleted != true);

                // 🔹 Filter by Id (if provided)
                if (dto.Id >  0)
                {
                    query = query.Where(x => x.Id == dto.Id);
                }

                // 🔹 Filter by Active status (true/false/null)
                if (dto.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == dto.IsActive.Value);
                }

                // ✅ 3️⃣ Execute Query
                var entities = await query.OrderByDescending(x => x.Id).ToListAsync();

                if (!entities.Any())
                {
                    _logger.LogWarning("No AssetCategory records found for TenantId: {TenantId}", dto.TenantId);
                    return new List<GetCategoryResponseDTO>();
                }

                // ✅ 4️⃣ Map and Return
                var result = _mapper.Map<List<GetCategoryResponseDTO>>(entities);

                _logger.LogInformation("Fetched {Count} AssetCategory records for TenantId: {TenantId}", result.Count, dto.TenantId);
                return result;
            }
            catch (Exception ex)
            {
                // ✅ 5️⃣ Exception Handling
                _logger.LogError(ex, "Error occurred while fetching AssetCategory records for TenantId: {TenantId}", dto?.TenantId);
                return new List<GetCategoryResponseDTO>();
            }
        }

        /// <summary>
        /// Adds a new Asset Category record for a given tenant.
        /// Includes validation for null input, duplicate category check, and soft delete flag.
        /// </summary>
        /// <param name="Dto">AddCategoryRequestDTO containing TenantId, CategoryName, EmployeeId, etc.</param>
        /// <returns>List of GetCategoryResponseDTO after successful insertion.</returns>
        public async Task<List<GetCategoryResponseDTO>> AddAsync(AddCategoryReqestDTO? Dto)
        {
            try
            {
                // ✅ 1️⃣ Basic validation
                if (Dto == null)
                {
                    _logger.LogWarning("AddAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(Dto), "Request data cannot be null.");
                }

                if (Dto.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId provided in AddAsync: {TenantId}", Dto.TenantId);
                    throw new ArgumentException("TenantId must be greater than zero.", nameof(Dto.TenantId));
                }

                if (string.IsNullOrWhiteSpace(Dto.CategoryName))
                {
                    _logger.LogWarning("CategoryName is missing in AddAsync request for TenantId: {TenantId}", Dto.TenantId);
                    throw new ArgumentException("CategoryName cannot be null or empty.", nameof(Dto.CategoryName));
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation(
                    "Attempting to insert new Asset Category for TenantId: {TenantId}, CategoryName: {CategoryName}",
                    Dto.TenantId, Dto.CategoryName);

                // ✅ 2️⃣ Duplicate check
                var existingCategory = await context.AssetCategories
                    .FirstOrDefaultAsync(ac =>
                        ac.TenantId == Dto.TenantId &&
                        ac.CategoryName.ToLower() == Dto.CategoryName.ToLower() &&
                        ac.IsSoftDeleted == false);

                if (existingCategory != null)
                {
                    _logger.LogWarning(
                        "Duplicate Asset Category detected for TenantId: {TenantId}, CategoryName: {CategoryName}",
                        Dto.TenantId, Dto.CategoryName);

                    throw new InvalidOperationException(
                        $"Category '{Dto.CategoryName}' already exists for TenantId {Dto.TenantId}.");
                }

                // ✅ 3️⃣ Map DTO → Entity
                var newCategory = new AssetCategory
                {
                    TenantId = Dto.TenantId,
                    CategoryName = Dto.CategoryName.Trim(),
                    Remark = Dto.Remark?.Trim(),
                    IsActive = Dto.IsActive,
                    IsSoftDeleted = false,
                    AddedById = Dto.EmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                // ✅ 4️⃣ Save record
                await context.AssetCategories.AddAsync(newCategory);
                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ Asset Category added successfully with Id: {Id}, TenantId: {TenantId}",
                    newCategory.Id, Dto.TenantId);

                // ✅ 5️⃣ Fetch updated list and map
                var categories = await GetAllAssetCategoryAsync(Dto.TenantId, Dto.IsActive);
                var mappedResponse = _mapper.Map<List<GetCategoryResponseDTO>>(categories);

                return mappedResponse.OrderByDescending(r => r.Id).ToList();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "⚠️ Null input detected while adding Asset Category.");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ Invalid input data while adding Asset Category for TenantId: {TenantId}", Dto?.TenantId);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "⚠️ Duplicate category detected for TenantId: {TenantId}", Dto?.TenantId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error occurred while adding Asset Category for TenantId: {TenantId}", Dto?.TenantId);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing Asset Category record for a specific tenant.
        /// Includes validation, duplicate check, and soft delete protection.
        /// </summary>
        /// <param name="dto">The DTO containing updated category data.</param>
        /// <returns>
        /// Returns the updated category as <see cref="GetCategoryResponseDTO"/> 
        /// if successful, or null if the record doesn't exist.
        /// </returns>
        public async Task<bool> UpdateAsync(UpdateCategoryReqestDTO? dto)
        {
            try
            {
                // ✅ 1️⃣ Input validation
                if (dto == null)
                {
                    _logger.LogWarning("UpdateAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Request object cannot be null.");
                }

                if (dto.Id <= 0)
                {
                    _logger.LogWarning("Invalid Category Id provided in UpdateAsync: {Id}", dto.Id);
                    throw new ArgumentException("Category Id must be greater than zero.", nameof(dto.Id));
                }

                if (dto.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId provided in UpdateAsync: {TenantId}", dto.TenantId);
                    throw new ArgumentException("TenantId must be greater than zero.", nameof(dto.TenantId));
                }

                

                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation(
                    "Attempting to update Asset Category. TenantId: {TenantId}, CategoryId: {Id}, NewName: {CategoryName}",
                    dto.TenantId, dto.Id, dto.CategoryName);

                // ✅ 2️⃣ Check if record exists
                var existingCategory = await context.AssetCategories
                    .FirstOrDefaultAsync(x =>
                        x.Id == dto.Id &&
                        x.TenantId == dto.TenantId &&
                        x.IsSoftDeleted != true);

                if (existingCategory == null)
                {
                    _logger.LogWarning(
                        "Asset Category not found for update. Id: {Id}, TenantId: {TenantId}",
                        dto.Id, dto.TenantId);
                    throw new InvalidOperationException($"Asset Category not found for update. Id: {dto.Id}, TenantId: {dto.TenantId}");
                }                                   
                
                existingCategory.CategoryName = !string.IsNullOrWhiteSpace(dto.CategoryName) ? dto.CategoryName : existingCategory.CategoryName;
                existingCategory.Remark = !string.IsNullOrWhiteSpace(dto.Remark) ? dto.Remark : existingCategory.Remark;

                if (dto.IsActive.HasValue)
                    existingCategory.IsActive = dto.IsActive.Value; // Only update if value provided
                
                   
                existingCategory.UpdatedById = dto.EmployeeId;
                existingCategory.UpdatedDateTime = DateTime.UtcNow;
                // ✅ 5️⃣ Save changes
                context.AssetCategories.Update(existingCategory);
              var isupdate=  await context.SaveChangesAsync();

                if (isupdate<=0)
                {
                    _logger.LogWarning(
                        "No changes were made while updating Asset Category. Id: {Id}, TenantId: {TenantId}",
                        dto.Id, dto.TenantId);
                    return false;
                }
                else
                {
                    _logger.LogInformation(
                        "Changes saved successfully while updating Asset Category. Id: {Id}, TenantId: {TenantId}",
                        dto.Id, dto.TenantId);
                    return true;
                }     
              
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "⚠️ Null input detected while updating Asset Category.");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ Invalid data while updating Asset Category for TenantId: {TenantId}", dto?.TenantId);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "⚠️ Duplicate category name during update for TenantId: {TenantId}", dto?.TenantId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error occurred while updating Asset Category Id: {Id}", dto?.Id);
                throw;
            }
        }


        /// <summary>
        /// Soft deletes an existing Asset Category record for a specific tenant.
        /// </summary>
        /// <param name="dto">The DTO containing category Id and tenant Id.</param>
        /// <returns>
        /// Returns true if the record was successfully soft-deleted, false if not found.
        /// </returns>
        public async Task<bool> DeleteAsync(DeleteCategoryReqestDTO dto)
        {
            try
            {
                // ✅ 1️⃣ Input Validation
                if (dto == null)
                {
                    _logger.LogWarning("DeleteAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Request object cannot be null.");
                }

                if (dto.Id <= 0)
                {
                    _logger.LogWarning("Invalid Category Id provided in DeleteAsync: {Id}", dto.Id);
                    throw new ArgumentException("Category Id must be greater than zero.", nameof(dto.Id));
                }

                if (dto.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId provided in DeleteAsync: {TenantId}", dto.TenantId);
                    throw new ArgumentException("TenantId must be greater than zero.", nameof(dto.TenantId));
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation(
                    "Attempting to delete Asset Category. TenantId: {TenantId}, CategoryId: {Id}",
                    dto.TenantId, dto.Id);

                // ✅ 2️⃣ Fetch category record
                var existingCategory = await context.AssetCategories
                    .FirstOrDefaultAsync(x =>
                        x.Id == dto.Id &&
                        x.TenantId == dto.TenantId &&
                        x.IsSoftDeleted == false);

                if (existingCategory == null)
                {
                    _logger.LogWarning(
                        "Asset Category not found for delete. Id: {Id}, TenantId: {TenantId}",
                        dto.Id, dto.TenantId);
                    return false;
                }

                // ✅ 3️⃣ Perform Soft Delete
                existingCategory.IsSoftDeleted = true;
                existingCategory.IsActive = false;
                existingCategory.SoftDeletedById = dto.EmployeeId;
                existingCategory.SoftDeletedDateTime = DateTime.UtcNow;

                // ✅ 4️⃣ Save Changes
                context.AssetCategories.Update(existingCategory);
                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ Asset Category soft deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    dto.Id, dto.TenantId);

                return true;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "⚠️ Null input detected while deleting Asset Category.");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ Invalid data while deleting Asset Category for TenantId: {TenantId}", dto?.TenantId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error occurred while deleting Asset Category Id: {Id}", dto?.Id);
                throw;
            }
        }


      

        public async Task<List<AssetCategory>> GetAllAssetCategoryAsync(long tenantId, bool isActive)
        {
            try
            {
                _logger.LogInformation("Fetching Asset Categories for TenantId: {TenantId}, IsActive: {IsActive}", tenantId, isActive);

                var categories = await _context.AssetCategories
                    .Where(ac => ac.TenantId == tenantId
                              && ac.IsActive == isActive
                              && (ac.IsSoftDeleted == ConstantValues.IsByDefaultFalse || ac.IsSoftDeleted == null))
                    .OrderByDescending(ac => ac.Id) // Latest sabse pehle
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Asset Categories for TenantId: {TenantId}", tenantId);
                throw; // bubble up
            }
        }
 
          
        }
    }

 


