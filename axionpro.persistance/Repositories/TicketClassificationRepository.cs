using AutoMapper;
 
using axionpro.application.DTOS.TicketDTO.Classification;
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
    public class TicketClassificationRepository : ITicketClassificationRepository
    {
        private readonly WorkforceDbContext _context;
       
        private readonly IMapper _mapper;
        private readonly ILogger<TicketClassificationRepository> _logger;

        public TicketClassificationRepository(
            WorkforceDbContext context,
            ILogger<TicketClassificationRepository> logger,
            IMapper mapper)
          
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            
        }

        //  ADD
        public async Task<GetClassificationResponseDTO> AddAsync(AddClassificationRequestDTO dto)
        {
            try
            {
                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                if (dto == null)
                    throw new ArgumentException("Request cannot be null.");

                if (string.IsNullOrWhiteSpace(dto.ClassificationName))
                    throw new ArgumentException("ClassificationName is required.");

                // ===============================
                // 2️⃣ DUPLICATE CHECK
                // ===============================
                var exists = await _context.TicketClassifications
                    .AnyAsync(x =>
                        x.TenantId == dto.Prop.TenantId &&
                        x.IsActive &&
                        (x.IsSoftDeleted != true) &&
                        x.ClassificationName.ToLower() == dto.ClassificationName.ToLower());

                if (exists)
                    throw new InvalidOperationException("Classification already exists.");

                // ===============================
                // 3️⃣ CREATE ENTITY (MANUAL)
                // ===============================
                var entity = new TicketClassification
                {
                    ClassificationName = dto.ClassificationName,
                    Description = dto.Description,

                    TenantId = dto.Prop.TenantId,

                    IsActive = dto.IsActive ?? true,
                    IsSoftDeleted = false,

                    AddedById = dto.Prop.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                // ===============================
                // 4️⃣ SAVE
                // ===============================
                await _context.TicketClassifications.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Ticket classification added. Id: {Id}", entity.Id);

                // ===============================
                // 5️⃣ RETURN DTO
                // ===============================
                return new GetClassificationResponseDTO
                {
                    Id = entity.Id,
                    ClassificationName = entity.ClassificationName,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                  
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding ticket classification.");
                throw;
            }
        }

        //  GET BY ID
        public async Task<PagedResponseDTO<GetClassificationResponseDTO?>> GetByIdAsync(GetClassificationRequestDTO dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid DTO or Id for GetByIdAsync.");
                    return null;
                }

               
                var entity = await _context.TicketClassifications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == dto.Id && x.IsActive && (x.IsSoftDeleted !=true));

                if (entity == null)
                {
                    _logger.LogWarning("⚠️ No classification found for Id: {Id}", dto.Id);
                    return null;
                }

                var response = _mapper.Map<PagedResponseDTO<GetClassificationResponseDTO>>(entity);
               
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching classification by Id: {Id}", dto.Id);
                return new PagedResponseDTO<GetClassificationResponseDTO>();
            }
        }

        //  GET ALL
        public class BaseRequest
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SortBy { get; set; }
            public string? SortOrder { get; set; } = "desc";
            public string? UserEmployeeId { get; set; }



        }

        public async Task<List<GetClassificationResponseDTO>> GetDDLAsync(bool isActive, long tenantId)
        {
            try
            {
                var result = await _context.TicketClassifications
                    .AsNoTracking()
                    .Where(x =>
                        x.TenantId == tenantId &&
                        x.IsActive == isActive &&
                        (x.IsSoftDeleted != true))
                    .OrderBy(x => x.ClassificationName)
                    .Select(x => new GetClassificationResponseDTO
                    {
                        Id = x.Id,
                        ClassificationName = x.ClassificationName,
                        IsActive = x.IsActive,
                        Description = x.Description

                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching classification DDL");
                throw;
            }
        }
        public async Task<PagedResponseDTO<GetClassificationResponseDTO>> GetAllAsync(GetAllClassificationRequestDTO dto)
        {
            try
            {
                // ===============================
                // 1️⃣ DEFAULT PAGINATION
                // ===============================
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                // ===============================
                // 2️⃣ BASE QUERY
                // ===============================
                var query = _context.TicketClassifications
                    .AsNoTracking()
                    .Where(x =>
                        x.TenantId == dto.Prop.TenantId &&
                        x.IsActive == true &&
                        (x.IsSoftDeleted != true));

                // ===============================
                // 3️⃣ TOTAL COUNT
                // ===============================
                var totalCount = await query.CountAsync();

                // ===============================
                // 4️⃣ SORTING
                // ===============================
                query = dto.SortBy?.ToLower() switch
                {
                    "classificationname" => dto.SortOrder == "asc"
                        ? query.OrderBy(x => x.ClassificationName)
                        : query.OrderByDescending(x => x.ClassificationName),

                    "addeddatetime" => dto.SortOrder == "asc"
                        ? query.OrderBy(x => x.AddedDateTime)
                        : query.OrderByDescending(x => x.AddedDateTime),

                    _ => query.OrderByDescending(x => x.Id)
                };

                // ===============================
                // 5️⃣ PAGINATION
                // ===============================
                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetClassificationResponseDTO
                    {
                        Id = x.Id,
                        ClassificationName = x.ClassificationName,
                        Description = x.Description,
                        IsActive = x.IsActive,
                      
                    })
                    .ToListAsync();

                // ===============================
                // 6️⃣ TOTAL PAGES
                // ===============================
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // ===============================
                // 7️⃣ RESPONSE
                // ===============================
                return new PagedResponseDTO<GetClassificationResponseDTO>
                {
                    Data = data,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasUploadedAll = pageNumber >= totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching classifications.");
                throw;
            }
        }

        //  UPDATE
        public async Task<GetClassificationResponseDTO?> UpdateAsync(UpdateClassificationRequestDTO dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid DTO or Id for UpdateAsync.");
                    return new GetClassificationResponseDTO();
                }

                
                var existing = await _context.TicketClassifications.FirstOrDefaultAsync(x => x.Id == dto.Id && (x.IsSoftDeleted!=true));
                if (existing == null)
                {
                    _logger.LogWarning("⚠️ Classification not found for update. Id: {Id}", dto.Id);
                    return new GetClassificationResponseDTO();
                }

                existing.ClassificationName = dto.ClassificationName ?? existing.ClassificationName;
                existing.Description = dto.Description ?? existing.Description;
                existing.IsActive = dto.IsActive ?? existing.IsActive;
                existing.UpdatedById = dto.Prop?.EmployeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                _context.TicketClassifications.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation(" Classification updated successfully. Id: {Id}", dto.Id);

                var response = _mapper.Map<GetClassificationResponseDTO>(existing);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while updating classification. Id: {Id}", dto.Id);
                return new GetClassificationResponseDTO();
            }
        }

        //  DELETE (Soft Delete)
        public async Task<bool> DeleteAsync(DeleteClassificationRequestDTO dto , long employeeId)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid DTO or Id for DeleteClassificationAsync.");
                    return false;
                }
 
                var entity = await _context.TicketClassifications.FirstOrDefaultAsync(x => x.Id == dto.Id && x.IsSoftDeleted !=true);
                if (entity == null)
                {
                    _logger.LogWarning("⚠️ Classification not found for deletion. Id: {Id}", dto.Id);
                    return false;
                }

                entity.IsActive = false;
                entity.IsSoftDeleted = true;
                entity.SoftDeletedById = employeeId;
                entity.SoftDeletedTime = DateTime.UtcNow;

                _context.TicketClassifications.Update(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("🗑️ Classification soft-deleted successfully. Id: {Id}", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while deleting classification. Id: {Id}", dto.Id);
                return false;
            }
        }

        public async Task<GetClassificationResponseDTO?> GetByIdAsync(long id, long tenantId)
        {
            try
            {
                var result = await _context.TicketClassifications
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == id &&
                        x.TenantId == tenantId &&
                        x.IsActive == true &&
                        (x.IsSoftDeleted != true))
                    .Select(x => new GetClassificationResponseDTO
                    {
                        Id = x.Id,
                        ClassificationName = x.ClassificationName,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        
                    })
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching classification by Id: {Id}", id);
                throw;
            }
        }
    }
}
