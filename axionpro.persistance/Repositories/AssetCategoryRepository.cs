using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data; 
namespace axionpro.persistance.Repositories
{
    public class AssetCategoryRepository : IAssetCategoryRepository
    {
        private readonly WorkforceDbContext _context;
       
        private readonly IMapper _mapper;
        private readonly ILogger<AssetCategoryRepository> _logger;

        public AssetCategoryRepository(
            WorkforceDbContext context,
            ILogger<AssetCategoryRepository> logger,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            
        }

        public async Task<bool> IsAssetCategoryDuplicate(AssetCategory asset)
        {
            try
            {
             

                return await _context.AssetCategories.AnyAsync(a =>
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
        public async Task<PagedResponseDTO<GetCategoryResponseDTO>> GetAllAsync(GetCategoryReqestDTO? dto)
        {
            try
            {
                // ✅ 1️⃣ VALIDATION
                if (dto == null)
                {
                    _logger.LogWarning("GetAllAsync called with null DTO.");
                    return new PagedResponseDTO<GetCategoryResponseDTO>();
                }

                if (dto.Prop?.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId provided: {TenantId}", dto?.Prop?.TenantId);
                    return new PagedResponseDTO<GetCategoryResponseDTO>();
                }

                // ✅ 2️⃣ DEFAULT PAGINATION (IMPORTANT 🔥)
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // ✅ 3️⃣ BASE QUERY
                IQueryable<AssetCategory> query = _context.AssetCategories
                    .AsNoTracking()
                    .Where(x => x.TenantId == dto.Prop.TenantId && x.IsSoftDeleted != true);

                // 🔹 FILTER: Id
                if (dto.Id > 0)
                {
                    query = query.Where(x => x.Id == dto.Id);
                }

                // 🔹 FILTER: IsActive
                if (dto.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == dto.IsActive.Value);
                }

                // ✅ 4️⃣ TOTAL COUNT (before paging)
                int totalCount = await query.CountAsync();

                if (totalCount == 0)
                {
                    _logger.LogWarning("No AssetCategory records found for TenantId: {TenantId}", dto.Prop.TenantId);

                    return new PagedResponseDTO<GetCategoryResponseDTO>(
                        new List<GetCategoryResponseDTO>(),
                        0,
                        pageNumber,
                        pageSize
                    );
                }

                // ✅ 5️⃣ SORTING (Dynamic)
                query = dto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(x => x.Id)
                    : query.OrderByDescending(x => x.Id);

                // ✅ 6️⃣ PAGINATION
                var pagedData = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetCategoryResponseDTO
                    {
                        Id = x.Id,
                        CategoryName = x.CategoryName,
                        Remark = x.Remark,
                        IsActive = x.IsActive,
                        HasMultipleUser= x.HasMultipleUser,
                    })
                    .ToListAsync();

                // ✅ 7️⃣ RESPONSE BUILD
                var response = new PagedResponseDTO<GetCategoryResponseDTO>(
                    pagedData,
                    totalCount,
                    pageNumber,
                    pageSize
                );

                _logger.LogInformation(
                    "Fetched {Count} AssetCategory records (Page: {PageNumber}, Size: {PageSize}) for TenantId: {TenantId}",
                    pagedData.Count,
                    pageNumber,
                    pageSize,
                    dto.Prop.TenantId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching AssetCategory records for TenantId: {TenantId}",
                    dto?.Prop?.TenantId);

                return new PagedResponseDTO<GetCategoryResponseDTO>();
            }
        }
        /// <summary>
        /// Adds a new Asset Category record for a given tenant.
        /// Includes validation for null input, duplicate category check, and soft delete flag.
        /// </summary>
        /// <param name="Dto">AddCategoryRequestDTO containing TenantId, CategoryName, EmployeeId, etc.</param>
        /// <returns>List of GetCategoryResponseDTO after successful insertion.</returns>
        public async Task<GetCategoryResponseDTO> AddAsync(AddCategoryReqestDTO dto)
        {
            try
            {
                // ===============================
                // 1️⃣ NULL VALIDATION
                // ===============================
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                if (dto.Prop == null)
                    throw new ArgumentException("Request context (Prop) is required.");

                if (string.IsNullOrWhiteSpace(dto.CategoryName))
                    throw new ArgumentException("Category name is required.");

                _logger.LogInformation(
                    "Attempting to insert Asset Category for TenantId: {TenantId}, CategoryName: {CategoryName}",
                    dto.Prop.TenantId, dto.CategoryName);

                var categoryName = dto.CategoryName.Trim();

           
                // ===============================
                // 3️⃣ CREATE ENTITY
                // ===============================
                var entity = new AssetCategory
                { 
                    Id = 0, // EF will auto-generate
                    TenantId = dto.Prop.TenantId,
                    CategoryName = categoryName,
                    Remark = dto.Remark?.Trim(),
                    IsActive = true,
                    HasMultipleUser = dto.HasMultipleUser,
                    IsSoftDeleted = false,
                    AddedById = dto.Prop.EmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    

                };

                // ===============================
                // 4️⃣ SAVE
                // ===============================
                await _context.AssetCategories.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Asset Category created successfully with Id: {Id}",
                    entity.Id);

                // ===============================
                // 5️⃣ RETURN DTO (NO EXTRA DB CALL)
                // ===============================
                return new GetCategoryResponseDTO
                {
                    Id = entity.Id, // 🔐 later encode karna (tumhare pattern ke hisaab se)
                    CategoryName = entity.CategoryName,
                    Remark = entity.Remark,
                    IsActive = entity.IsActive,
                    HasMultipleUser = (bool)entity.HasMultipleUser
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while adding Asset Category for TenantId: {TenantId}",
                    dto?.Prop?.TenantId);

                throw;
            }
        }


        // 🔹 DUPLICATE CHECK (UPDATE SAFE)
        public async Task<bool> ExistsAsync(
            long tenantId,
            string categoryName,
            long? excludeId = null)
        {
            
            return await _context.AssetCategories.AnyAsync(x =>
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
            entity.HasMultipleUser = dto.HasMultipleUser ;
            entity.UpdatedById = dto.Prop.UserEmployeeId;
            entity.UpdatedDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 DELETE (SOFT)
        public async Task<bool> DeleteAsync(DeleteCategoryReqestDTO dto)
        {
            

            var entity = await _context.AssetCategories.FirstOrDefaultAsync(x =>
                x.Id == dto.Id &&
                x.TenantId == dto.Prop.TenantId &&
                x.IsSoftDeleted == false);

            if (entity == null)
                return false;

            entity.IsSoftDeleted = true;
            entity.IsActive = false;
            entity.SoftDeletedById = dto.Prop.EmployeeId;
            entity.SoftDeletedDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
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
                              && (ac.IsSoftDeleted !=true))
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

 


