using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    /// <summary>
    /// Repository for managing AssetStatus entity operations
    /// (Add, Update, Delete, GetAll) for Tenant context.
    /// </summary>
    public class AssetStatusRepository : IAssetStatusRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetStatusRepository> _logger;

        public AssetStatusRepository(
            WorkforceDbContext context,
            ILogger<AssetStatusRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }

        #region AssetStatus CRUD

        /// <summary>
        /// Adds a new AssetStatus record for a tenant.
        /// </summary>
        public async Task<GetStatusResponseDTO?> AddAsync(CreateStatusRequestDTO? assetStatus)
        {
            if (assetStatus == null)
            {
                _logger.LogWarning("AddAsync called with null AddStatusRequestDTO.");
                return null;
            }

            try
            {

                var entity = _mapper.Map<AssetStatus>(assetStatus);
                await using var context = await _contextFactory.CreateDbContextAsync();

                entity.AddedDateTime = DateTime.Now;
                entity.AddedById = assetStatus.EmployeeId;
                entity.IsSoftDeleted = false;

                await context.AssetStatuses.AddAsync(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "AssetStatus added successfully. TenantId: {TenantId}, StatusName: {StatusName}",
                    assetStatus.TenantId, assetStatus.StatusName);

                return _mapper.Map<GetStatusResponseDTO>(entity);
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
            await using var context = await _contextFactory.CreateDbContextAsync();

            if (requestDTO == null)
            {
                _logger.LogWarning("DeleteAsync called with null DeleteStatusRequestDTO.");
                return false;
            }

            try
            {
                var existingStatus = await context.AssetStatuses
                    .FirstOrDefaultAsync(x =>
                        x.Id == requestDTO.Id &&
                        x.TenantId == requestDTO.TenantId &&
                        x.IsSoftDeleted !=true);

                if (existingStatus == null)
                {
                    _logger.LogWarning(
                        "AssetStatus not found or already deleted. Id: {Id}, TenantId: {TenantId}",
                        requestDTO.Id, requestDTO.TenantId);
                    return false;
                }

                // Soft delete
                existingStatus.IsSoftDeleted = true;
                existingStatus.IsActive = false;
                existingStatus.SoftDeletedById = requestDTO.EmployeeId;
                existingStatus.DeletedDateTime = DateTime.Now;
                existingStatus.UpdatedById = requestDTO.EmployeeId;
                existingStatus.UpdatedDateTime = DateTime.Now;

                _context.AssetStatuses.Update(existingStatus);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "AssetStatus soft deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    requestDTO.Id, requestDTO.TenantId);

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
        public async Task<List<GetStatusResponseDTO>> GetAllAsync(GetStatusRequestDTO? assetStatus)
        {
            if (assetStatus == null || assetStatus.TenantId <= 0)
            {
                _logger.LogWarning("GetAllAsync called with null or invalid TenantId.");
                return new List<GetStatusResponseDTO>();
            }

            try
            {
                IQueryable<AssetStatus> query = _context.AssetStatuses
                    .Where(a => a.IsSoftDeleted != true &&
                                a.TenantId == assetStatus.TenantId);

                // Optional: filter by IsActive
                if (assetStatus.IsActive != null)
                    query = query.Where(a => a.IsActive == assetStatus.IsActive);

                // Filter by Id if provided, else fetch all
                if (assetStatus.Id>0)
                    query = query.Where(a => a.Id == assetStatus.Id);

                var entities = await query
                    .OrderByDescending(a => a.AddedDateTime)
                    .ToListAsync();

                _logger.LogInformation(
                    "Fetched {Count} AssetStatus records for TenantId: {TenantId}",
                    entities.Count, assetStatus.TenantId);

                // Map entities to DTOs
                return _mapper.Map<List<GetStatusResponseDTO>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching AssetStatus records for TenantId: {TenantId}",
                    assetStatus.TenantId);
                return new List<GetStatusResponseDTO>();
            }
        }


        public async Task<bool> UpdateAsync(UpdateStatusRequestDTO assetStatus)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            if (assetStatus == null || assetStatus.TenantId <= 0 || assetStatus.Id <= 0)
            {
                _logger.LogWarning("UpdateAssetStatusByTenantAsync called with invalid input. TenantId: {TenantId}, Id: {Id}",
                    assetStatus?.TenantId, assetStatus?.Id);
                // Return a default instance to satisfy non-nullable contract
                return false;
            }

            try
            {
                var existingStatus = await context.AssetStatuses
                    .FirstOrDefaultAsync(x =>
                        x.Id == assetStatus.Id &&
                        x.TenantId == assetStatus.TenantId &&
                        x.IsSoftDeleted !=true);

                if (existingStatus == null)
                {
                    _logger.LogWarning("AssetStatus not found. Id: {Id}, TenantId: {TenantId}",
                        assetStatus.Id, assetStatus.TenantId);
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
                existingStatus.UpdatedById = assetStatus.EmployeeId;
                existingStatus.UpdatedDateTime = DateTime.Now;

                _context.AssetStatuses.Update(existingStatus);
               var change= await context.SaveChangesAsync();
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
                    assetStatus.Id, assetStatus.TenantId);
                throw;
            }
        }


        /// <summary>
        /// Retrieves AssetStatus records mapped to GetStatusResponseDTO for tenant.
        /// </summary>







        #endregion
    }
}
