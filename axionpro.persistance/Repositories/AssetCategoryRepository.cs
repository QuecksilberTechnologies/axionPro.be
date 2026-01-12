using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
 

using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IQRService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using MediatR;
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
             //   await using var _context = await _contextFactory.CreateDbContextAsync();

                // ✅ 1️⃣ Basic Validation Checks
                if (dto == null)
                {
                    _logger.LogWarning("GetAllAsync called with null DTO.");
                    return new List<GetCategoryResponseDTO>();
                }

                if (dto.Prop.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId provided: {TenantId}", dto.Prop.TenantId);
                    return new List<GetCategoryResponseDTO>();
                }

                // ✅ 2️⃣ Build Query with Filters
                IQueryable<AssetCategory> query = _context.AssetCategories
                    .Where(x => x.TenantId == dto.Prop.TenantId && x.IsSoftDeleted != true);

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
                    _logger.LogWarning("No AssetCategory records found for TenantId: {TenantId}", dto.Prop.TenantId);
                    return new List<GetCategoryResponseDTO>();
                }

                // ✅ 4️⃣ Map and Return
                var result = _mapper.Map<List<GetCategoryResponseDTO>>(entities);

                _logger.LogInformation("Fetched {Count} AssetCategory records for TenantId: {TenantId}", result.Count, dto.Prop.TenantId);
                return result;
            }
            catch (Exception ex)
            {
                // ✅ 5️⃣ Exception Handling
                _logger.LogError(ex, "Error occurred while fetching AssetCategory records for TenantId: {TenantId}", dto?.Prop.TenantId);
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
                  
                _logger.LogInformation(
                    "Attempting to insert new Asset Category for TenantId: {TenantId}, CategoryName: {CategoryName}",
                    Dto.Prop.TenantId, Dto.CategoryName);

                // ✅ 2️⃣ Duplicate check
                var existingCategory = await _context.AssetCategories
                    .FirstOrDefaultAsync(ac =>
                        ac.TenantId == Dto.Prop.TenantId &&
                        ac.CategoryName.ToLower() == Dto.CategoryName.ToLower() &&
                        ac.IsSoftDeleted == false);

                if (existingCategory != null)
                {
                    _logger.LogWarning(
                        "Duplicate Asset Category detected for TenantId: {TenantId}, CategoryName: {CategoryName}",
                        Dto.Prop.TenantId, Dto.CategoryName);

                    throw new InvalidOperationException(
                        $"Category '{Dto.CategoryName}' already exists for TenantId {Dto.Prop.TenantId}.");
                }

                // ✅ 3️⃣ Map DTO → Entity
                var newCategory = new AssetCategory
                {
                    TenantId = Dto.Prop.TenantId,
                    CategoryName = Dto.CategoryName.Trim(),
                    Remark = Dto.Remark?.Trim(),
                   // IsActive = Dto.IsActive,
                    IsActive =true,
                    IsSoftDeleted = false,
                    AddedById = Dto.Prop.EmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                // ✅ 4️⃣ Save record
                await _context.AssetCategories.AddAsync(newCategory);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "✅ Asset Category added successfully with Id: {Id}, TenantId: {TenantId}",
                    newCategory.Id, Dto.Prop.TenantId);

                // ✅ 5️⃣ Fetch updated list and map
                var categories = await GetAllAssetCategoryAsync(Dto.Prop.TenantId, Dto.Prop.IsActive);
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
                _logger.LogWarning(ex, "⚠️ Invalid input data while adding Asset Category for TenantId: {TenantId}", Dto?.Prop.TenantId);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "⚠️ Duplicate category detected for TenantId: {TenantId}", Dto?.Prop.TenantId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error occurred while adding Asset Category for TenantId: {TenantId}", Dto?.Prop.TenantId);
                throw;
            }
        }
 

        // 🔹 DUPLICATE CHECK (UPDATE SAFE)
        public async Task<bool> ExistsAsync(
            long tenantId,
            string categoryName,
            long? excludeId = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.AssetCategories.AnyAsync(x =>
                x.TenantId == tenantId &&
                x.CategoryName.ToLower() == categoryName.ToLower() &&
                x.IsSoftDeleted == false &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // 🔹 UPDATE
        public async Task<bool> UpdateAsync(UpdateCategoryReqestDTO dto)
        {
      

            var entity = await _context.AssetCategories.FirstOrDefaultAsync(x =>
                x.Id == dto.Id &&
                x.TenantId == dto.Prop.TenantId &&
                x.IsSoftDeleted == false);

            if (entity == null)
                return false;

            entity.CategoryName = dto.CategoryName.Trim();
            entity.Remark = dto.Remark?.Trim();
            entity.IsActive = dto.IsActive ?? entity.IsActive;
            entity.UpdatedById = dto.Prop.UserEmployeeId;
            entity.UpdatedDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 DELETE (SOFT)
        public async Task<bool> DeleteAsync(DeleteCategoryReqestDTO dto)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.AssetCategories.FirstOrDefaultAsync(x =>
                x.Id == dto.Id &&
                x.TenantId == dto.Prop.TenantId &&
                x.IsSoftDeleted == false);

            if (entity == null)
                return false;

            entity.IsSoftDeleted = true;
            entity.IsActive = false;
            entity.SoftDeletedById = dto.Prop.EmployeeId;
            entity.SoftDeletedDateTime = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
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

 


