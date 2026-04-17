using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Interfaces.IRepositories;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using axionpro.domain.Entity;
using axionpro.application.DTOS.Pagination;

namespace axionpro.persistance.Repositories
{
    /// <summary>
    /// Repository for managing AssetStatus entity operations
    /// (Add, Update, Delete, GetAll) for Tenant context.
    /// </summary>
    public class AssetStatusRepository : IAssetStatusRepository
    {
        private readonly WorkforceDbContext _context;
       
        private readonly IMapper _mapper;
        private readonly ILogger<AssetStatusRepository> _logger;

        public AssetStatusRepository(
            WorkforceDbContext context,
            ILogger<AssetStatusRepository> logger,
            IMapper mapper
           )
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            
        }

        #region AssetStatus CRUD

        /// <summary>
        /// Adds a new AssetStatus record for a tenant.
        /// </summary>
        public async Task<GetStatusResponseDTO?> AddAsync(AssetStatus assetStatus)
        {
            if (assetStatus == null)
            {
                _logger.LogWarning("AddAsync called with null AddStatusRequestDTO.");
                return null;
            }

            try
            {                 

                await _context.AssetStatuses.AddAsync(assetStatus);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "AssetStatus added successfully. TenantId: {TenantId}, StatusName: {StatusName}",
                    assetStatus.TenantId, assetStatus.StatusName);

                return _mapper.Map<GetStatusResponseDTO>(assetStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding AssetStatus for TenantId: {TenantId}", assetStatus.TenantId);
                throw;
            }
        }
         

        /// <summary>
        /// Soft deletes an AssetStatus record.
        /// </summary>
        public async Task<bool> DeleteAsync(DeleteStatusReqestDTO requestDTO)
        {
           
            if (requestDTO == null)
            {
                _logger.LogWarning("DeleteAsync called with null DeleteStatusRequestDTO.");
                return false;
            }

            try
            {
                var existingStatus = await _context.AssetStatuses
                    .FirstOrDefaultAsync(x =>
                        x.Id == requestDTO.Id &&
                        x.TenantId == requestDTO.Prop.TenantId &&
                        x.IsSoftDeleted !=true);

                if (existingStatus == null)
                {
                    _logger.LogWarning(
                        "AssetStatus not found or already deleted. Id: {Id}, TenantId: {TenantId}",
                        requestDTO.Id, requestDTO.Prop.TenantId);
                    return false;
                }

                // Soft delete
                existingStatus.IsSoftDeleted = true;
                existingStatus.IsActive = false;
                existingStatus.SoftDeletedById = requestDTO.Prop.EmployeeId;
                existingStatus.DeletedDateTime = DateTime.UtcNow;
                existingStatus.UpdatedById = requestDTO.Prop.EmployeeId;
                existingStatus.UpdatedDateTime = DateTime.UtcNow; ;

                this._context.AssetStatuses.Update(existingStatus);
                await this._context.SaveChangesAsync();

                _logger.LogInformation(
                    "AssetStatus soft deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    requestDTO.Id, requestDTO.Prop.TenantId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while soft deleting AssetStatus Id: {Id}", requestDTO.Id);
                throw;
            }
        }


        /// <summary>
        /// Retrieves all AssetStatus records for a tenant with optional isActive filter.
        /// </summary>
        ///   public 
        public async Task<PagedResponseDTO<GetStatusResponseDTO>> GetAllAsync(GetStatusRequestDTO? dto)
        {
            try
            {
                // ✅ 1️⃣ VALIDATION
                if (dto == null)
                {
                    _logger.LogWarning("GetAllAsync called with null DTO.");
                    return new PagedResponseDTO<GetStatusResponseDTO>();
                }

                if (dto.Prop?.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId: {TenantId}", dto?.Prop?.TenantId);
                    return new PagedResponseDTO<GetStatusResponseDTO>();
                }

                // ✅ 2️⃣ DEFAULT PAGINATION
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // ✅ 3️⃣ BASE QUERY
                IQueryable<AssetStatus> query = _context.AssetStatuses
                    .AsNoTracking()
                    .Where(a => a.IsSoftDeleted != true &&
                                a.TenantId == dto.Prop.TenantId);

                // 🔹 FILTER: IsActive
                if (dto.IsActive)
                    query = query.Where(a => a.IsActive == dto.IsActive && a.IsSoftDeleted !=true);

                // 🔹 FILTER: Id
                if (dto.Id > 0)
                    query = query.Where(a => a.Id == dto.Id);

                // ✅ 4️⃣ TOTAL COUNT
                int totalCount = await query.CountAsync();

                if (totalCount == 0)
                {
                    _logger.LogWarning(
                        "No AssetStatus records found for TenantId: {TenantId}",
                        dto.Prop.TenantId);

                    return new PagedResponseDTO<GetStatusResponseDTO>(
                        new List<GetStatusResponseDTO>(),
                        0,
                        pageNumber,
                        pageSize
                    );
                }

                // ✅ 5️⃣ SORTING
                query = query.OrderByDescending(a => a.AddedDateTime);

                // ✅ 6️⃣ PAGINATION + PROJECTION
                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new GetStatusResponseDTO
                    {
                        Id = a.Id,
                        StatusName = a.StatusName,
                        Description = a.Description,
                        IsActive = a.IsActive ?? false,
                        ColorKey = a.ColorKey
                         
                    })
                    .ToListAsync();

                // ✅ 7️⃣ RESPONSE
                var response = new PagedResponseDTO<GetStatusResponseDTO>(
                    data,
                    totalCount,
                    pageNumber,
                    pageSize
                );

                _logger.LogInformation(
                    "Fetched {Count} AssetStatus records (Page: {PageNumber}, Size: {PageSize}) for TenantId: {TenantId}",
                    data.Count,
                    pageNumber,
                    pageSize,
                    dto.Prop.TenantId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching AssetStatus records for TenantId: {TenantId}",
                    dto?.Prop?.TenantId);

                return new PagedResponseDTO<GetStatusResponseDTO>();
            }
        }


        public async Task<AssetStatus?> GetByIdAsync(int? id)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching AssetStatus with Id={AssetStatusId}", id);

                var assetStatus = await _context.AssetStatuses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a =>
                        a.Id == id &&
                        a.IsSoftDeleted != true);

                return assetStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error occurred while fetching AssetStatus with Id={AssetStatusId}",
                    id);

                throw;
            }
        }


        public async Task<bool> UpdateAsync(UpdateStatusRequestDTO assetStatus)
        {
          
            if (assetStatus == null || assetStatus.Prop.TenantId <= 0 || assetStatus.Id <= 0)
            {
                _logger.LogWarning("UpdateAssetStatusByTenantAsync called with invalid input. TenantId: {TenantId}, Id: {Id}",
                    assetStatus?.Prop.TenantId, assetStatus?.Id);
                // Return a default instance to satisfy non-nullable contract
                return false;
            }

            try
            {
                var existingStatus = await _context.AssetStatuses
                    .FirstOrDefaultAsync(x =>
                        x.Id == assetStatus.Id &&
                        x.TenantId == assetStatus.Prop.TenantId &&
                        x.IsSoftDeleted !=true);

                if (existingStatus == null)
                {
                    _logger.LogWarning("AssetStatus not found. Id: {Id}, TenantId: {TenantId}",
                        assetStatus.Id, assetStatus.Prop.TenantId);
                    // Return a default instance to satisfy non-nullable contract
                    return false;
                }

                existingStatus.StatusName = assetStatus.StatusName ?? existingStatus.StatusName;
                existingStatus.Description = assetStatus.Description ?? existingStatus.Description;
                existingStatus.ColorKey = assetStatus.ColorKey ?? existingStatus.ColorKey;
                // Update IsActive only if value provided
                if (assetStatus.IsActive.HasValue)
                {
                    existingStatus.IsActive = assetStatus.IsActive.Value;
                }
                existingStatus.UpdatedById = assetStatus.Prop.EmployeeId;
                existingStatus.UpdatedDateTime = DateTime.UtcNow;

                this._context.AssetStatuses.Update(existingStatus);
               var change= await _context.SaveChangesAsync();
                 if(change > 0)
                 {
                    return true;
                 }
                 else
                 {
                    return false;
                }   
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating AssetStatus Id: {Id}, TenantId: {TenantId}",
                    assetStatus.Id, assetStatus.Prop.TenantId);
                throw;
            }
        }


        /// <summary>
        /// Retrieves AssetStatus records mapped to GetStatusResponseDTO for tenant.
        /// </summary>







        #endregion
    }
}
