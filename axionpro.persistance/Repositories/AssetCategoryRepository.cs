using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Interfaces.IRepositories;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace axionpro.persistance.Repositories
{
    public class AssetCategoryRepository : IAssetCategoryRepository
    {
        private readonly WorkforcedbContext _context;
        private readonly IDbContextFactory<WorkforcedbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetCategoryRepository> _logger;

        public AssetCategoryRepository(
            WorkforcedbContext context,
            ILogger<AssetCategoryRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforcedbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }

        public async Task<bool> IsAssetCategoryDuplicate(Assetcategory asset)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                return await context.Assetcategories.AnyAsync(a =>
                    (a.Categoryname == asset.Categoryname));
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
                IQueryable<Assetcategory> query = _context.Assetcategories
                    .Where(x => x.Tenantid == dto.Prop.TenantId && x.Issoftdeleted != true);

                // 🔹 Filter by Id (if provided)
                if (dto.Id >  0)
                {
                    query = query.Where(x => x.Id == dto.Id);
                }

                // 🔹 Filter by Active status (true/false/null)
                if (dto.IsActive.HasValue)
                {
                    query = query.Where(x => x.Isactive == dto.IsActive.Value);
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
                var existingCategory = await _context.Assetcategories
                    .FirstOrDefaultAsync(ac =>
                        ac.Tenantid == Dto.Prop.TenantId &&
                        ac.Categoryname.ToLower() == Dto.CategoryName.ToLower() &&
                        ac.Issoftdeleted == false);

                if (existingCategory != null)
                {
                    _logger.LogWarning(
                        "Duplicate Asset Category detected for TenantId: {TenantId}, CategoryName: {CategoryName}",
                        Dto.Prop.TenantId, Dto.CategoryName);

                    throw new InvalidOperationException(
                        $"Category '{Dto.CategoryName}' already exists for TenantId {Dto.Prop.TenantId}.");
                }

                // ✅ 3️⃣ Map DTO → Entity
                var newCategory = new Assetcategory
                {
                    Tenantid = Dto.Prop.TenantId,
                    Categoryname = Dto.CategoryName.Trim(),
                    Remark = Dto.Remark?.Trim(),
                   // IsActive = Dto.IsActive,
                    Isactive =true,
                    Issoftdeleted = false,
                    Addedbyid = Dto.Prop.EmployeeId,
                    Addeddatetime = DateTime.UtcNow
                };

                // ✅ 4️⃣ Save record
                await _context.Assetcategories.AddAsync(newCategory);
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

            return await context.Assetcategories.AnyAsync(x =>
                x.Tenantid == tenantId &&
                x.Categoryname.ToLower() == categoryName.ToLower() &&
                x.Issoftdeleted == false &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // 🔹 UPDATE
        public async Task<bool> UpdateAsync(UpdateCategoryReqestDTO dto)
        {
      

            var entity = await _context.Assetcategories.FirstOrDefaultAsync(x =>
                x.Id == dto.Id &&
                x.Tenantid == dto.Prop.TenantId &&
                x.Issoftdeleted == false);

            if (entity == null)
                return false;

            entity.Categoryname = dto.CategoryName.Trim();
            entity.Remark = dto.Remark?.Trim();
            entity.Isactive = dto.IsActive ?? entity.Isactive;
            entity.Updatedbyid = dto.Prop.UserEmployeeId;
            entity.Updateddatetime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 DELETE (SOFT)
        public async Task<bool> DeleteAsync(DeleteCategoryReqestDTO dto)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.Assetcategories.FirstOrDefaultAsync(x =>
                x.Id == dto.Id &&
                x.Tenantid == dto.Prop.TenantId &&
                x.Issoftdeleted == false);

            if (entity == null)
                return false;

            entity.Issoftdeleted = true;
            entity.Isactive = false;
            entity.Softdeletedbyid = dto.Prop.EmployeeId;
            entity.Softdeleteddatetime = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }



        public async Task<List<Assetcategory>> GetAllAssetCategoryAsync(long tenantId, bool isActive)
        {
            try
            {
                _logger.LogInformation("Fetching Asset Categories for TenantId: {TenantId}, IsActive: {IsActive}", tenantId, isActive);

                var categories = await _context.Assetcategories
                    .Where(ac => ac.Tenantid == tenantId
                              && ac.Isactive == isActive
                              && (ac.Issoftdeleted == ConstantValues.IsByDefaultFalse || ac.Issoftdeleted == null))
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

 


