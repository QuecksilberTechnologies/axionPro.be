using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Header;
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
    public class TicketHeaderRepository : ITicketHeaderRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketHeaderRepository> _logger;

        public TicketHeaderRepository(
            WorkforceDbContext context,
            ILogger<TicketHeaderRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }

        // ✅ ADD
        public async Task<List<GetHeaderResponseDTO>> AddAsync(AddHeaderRequestDTO dto)
        {
            var result = new List<GetHeaderResponseDTO>();

            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ AddAsync called with null DTO.");
                    return result;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Duplicate check
                bool exists = await context.TicketHeaders
                    .AnyAsync(x => x.HeaderName.ToLower() == dto.HeaderName.ToLower()
                                && x.IsActive
                                && (x.IsSoftDeleted == null || x.IsSoftDeleted == false)
                                && x.TenantId == dto.TenantId);

                if (exists)
                {
                    _logger.LogWarning("⚠️ Ticket header already exists: {Name}", dto.HeaderName);
                    // ✅ Return existing list instead of empty
                    var existingHeaders = await context.TicketHeaders
                        .Where(x => x.TenantId == dto.TenantId && x.IsActive && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                        .ToListAsync();
                    return _mapper.Map<List<GetHeaderResponseDTO>>(existingHeaders);
                }

                // ✅ Map DTO → Entity
                var entity = _mapper.Map<TicketHeader>(dto);
                entity.AddedById = dto.EmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;

                // ✅ Add to DB
                await context.TicketHeaders.AddAsync(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Ticket header added successfully. Id: {Id}", entity.Id);

                // ✅ Fetch all headers for this tenant after adding
                var allHeaders = await context.TicketHeaders
                    .Where(x => x.TenantId == dto.TenantId && x.IsActive && (x.IsSoftDeleted == null || x.IsSoftDeleted == false))
                    .ToListAsync();

                result = _mapper.Map<List<GetHeaderResponseDTO>>(allHeaders);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while adding ticket header.");
                return result;
            }
        }


        // ✅ GET BY ID
        /// <summary>
        /// Get header by Id
        /// </summary>
        public async Task<List<GetHeaderResponseDTO>> GetByIdAsync(GetHeaderRequestDTO dto)
        {
            var result = new List<GetHeaderResponseDTO>();

            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid DTO or Id for GetByIdAsync.");
                  
                    return result;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                var entity = await context.TicketHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == dto.Id && x.IsActive && (x.IsSoftDeleted == null || x.IsSoftDeleted == false));

                if (entity == null)
                {
                    _logger.LogWarning("⚠️ Ticket header not found for Id: {Id}", dto.Id);
                    return new List<GetHeaderResponseDTO>();

                }

                var response = _mapper.Map<GetHeaderResponseDTO>(entity);
               

                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching ticket header by Id: {Id}", dto.Id);
                return new List<GetHeaderResponseDTO>();

            }
        }

        // TODO: Add other repository functions (AddAsync, UpdateAsync, DeleteAsync, GetAllAsync) in similar style



        // ✅ GET ALL
        public async Task<List<GetHeaderResponseDTO>> GetAllAsync(GetHeaderRequestDTO dto)
        {
            var result = new List<GetHeaderResponseDTO>();

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Step 1: Base Query
                var query = context.TicketHeaders
                    .Where(x => (x.IsSoftDeleted == null || x.IsSoftDeleted == false) && x.TenantId ==dto.TenantId)
                    .AsQueryable();                

                // ✅ Step 3: Optional Filters
                if (dto.Id > 0)
                    query = query.Where(x => x.Id == dto.Id);

                if (!string.IsNullOrEmpty(dto.HeaderName))
                    query = query.Where(x => x.HeaderName.ToLower().Contains(dto.HeaderName.ToLower()));

                if (!string.IsNullOrEmpty(dto.Description))
                    query = query.Where(x => x.Description.ToLower().Contains(dto.Description.ToLower()));

                if (dto.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == dto.IsActive.Value);

                if (dto.TicketClassificationId.HasValue && dto.TicketClassificationId.Value > 0)
                    query = query.Where(x => x.TicketClassificationId == dto.TicketClassificationId.Value);

                if (dto.IsAssetRelated.HasValue)
                    query = query.Where(x => x.IsAssetRelated == dto.IsAssetRelated.Value);

                // (Optional) Role-based filtering example (if needed)
                if (dto.RoleId > 0)
                {
                    _logger.LogInformation("🔑 Applying RoleId filter: {RoleId}", dto.RoleId);
                    // yahan agar koi Role-based header visibility logic ho, to wo lagao
                }

                // ✅ Step 4: Fetch
                var headers = await query.AsNoTracking().ToListAsync();

                // ✅ Step 5: Map to DTO
                result = _mapper.Map<List<GetHeaderResponseDTO>>(headers);

                // ✅ Step 6: Add success message
                

                _logger.LogInformation("✅ {Count} headers fetched successfully for TenantId={TenantId}", result.Count, dto.TenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching filtered ticket headers.");

                return new List<GetHeaderResponseDTO>();
            };
             
        }

        // ✅ UPDATE
        public async Task<GetHeaderResponseDTO?> UpdateAsync(UpdateHeaderRequestDTO dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid DTO or Id for UpdateAsync.");
                    return new GetHeaderResponseDTO();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();
                
                var existing = await context.TicketHeaders.FirstOrDefaultAsync(x => x.Id == dto.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false));
                if (existing == null)
                {
                    _logger.LogWarning("⚠️ Ticket header not found for update. Id: {Id}", dto.Id);
                    return new GetHeaderResponseDTO();

                }

                existing.HeaderName = dto.HeaderName ?? existing.HeaderName;
                existing.Description = dto.Description ?? existing.Description;
                existing.IsActive = dto.IsActive ?? existing.IsActive;
                existing.UpdatedById = dto.EmployeeId;
               
                existing.UpdatedDateTime = DateTime.UtcNow;
                existing.TicketClassificationId =
                   (dto.TicketClassificationId.HasValue && dto.TicketClassificationId.Value != 0)
                             ? dto.TicketClassificationId.Value
                             : existing.TicketClassificationId;

                context.TicketHeaders.Update(existing);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Ticket header updated successfully. Id: {Id}", dto.Id);

                var response = _mapper.Map<GetHeaderResponseDTO>(existing);
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while updating ticket header. Id: {Id}", dto.Id);
                return new GetHeaderResponseDTO();

            }
        }

        // ✅ DELETE (Soft Delete)
        public async Task<bool> DeleteAsync(DeleteHeaderRequestDTO dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid DTO or Id for DeleteHeaderAsync.");
                    return false;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                var entity = await context.TicketHeaders.FirstOrDefaultAsync(x => x.Id == dto.Id && (x.IsSoftDeleted == null || x.IsSoftDeleted == false));
                if (entity == null)
                {
                    _logger.LogWarning("⚠️ Ticket header not found for deletion. Id: {Id}", dto.Id);
                    return false;
                }

                entity.IsActive = false;
                entity.IsSoftDeleted = true;
                entity.SoftDeletedById = dto.EmployeeId;
                entity.SoftDeletedTime = DateTime.UtcNow;

                context.TicketHeaders.Update(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation("🗑️ Ticket header soft-deleted successfully. Id: {Id}", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while deleting ticket header. Id: {Id}", dto.Id);
                return false;
            }
        }
    }
}
