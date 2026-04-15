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
                {
                    _logger.LogWarning("⚠️ AddAsync called with null DTO.");
                    throw new ArgumentException("Request cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(dto.ClassificationName))
                    throw new ArgumentException("ClassificationName is required.");

                // ===============================
                // 2️⃣ DUPLICATE CHECK (SAFE)
                // ===============================
                bool exists = await _context.TicketClassifications
                    .AnyAsync(x =>
                        x.TenantId == dto.TenantId &&
                        x.IsActive &&
                        (x.IsSoftDeleted !=true) &&
                        x.ClassificationName.ToLower() == dto.ClassificationName.ToLower()
                    );

                if (exists)
                {
                    _logger.LogWarning("⚠️ Classification already exists: {Name}", dto.ClassificationName);
                    throw new InvalidOperationException("Classification already exists.");
                }

                // ===============================
                // 3️⃣ MAP DTO → ENTITY
                // ===============================
                var entity = _mapper.Map<TicketClassification>(dto);

                entity.TenantId = dto.TenantId;
                entity.AddedById = dto.EmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;

                entity.IsActive = dto.IsActive ?? true;
                entity.IsSoftDeleted = false;

                // ===============================
                // 4️⃣ SAVE
                // ===============================
                await _context.TicketClassifications.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Ticket classification added. Id: {Id}", entity.Id);

                // ===============================
                // 5️⃣ RETURN SINGLE DTO
                // ===============================
                var result = _mapper.Map<GetClassificationResponseDTO>(entity);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding ticket classification.");
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
  
        public async Task<PagedResponseDTO<GetClassificationResponseDTO>> GetAllAsync(GetClassificationRequestDTO dto)
        {
            var result = new List<GetClassificationResponseDTO>();
            try
            {
                
                var query = _context.TicketClassifications.AsQueryable();

               
                    query = query.Where(x =>   x.TenantId == dto.Prop.TenantId && (x.IsSoftDeleted!=true)&& x.IsActive == true);

                var list = await query.AsNoTracking().ToListAsync();

                result = _mapper.Map<List<GetClassificationResponseDTO>>(list);

                _logger.LogInformation(" {Count} classifications fetched successfully.", result.Count);

                var response = new PagedResponseDTO<GetClassificationResponseDTO>
                {
                    Data = result,
                    TotalCount = result.Count,
                    PageNumber = 1,
                    PageSize = result.Count
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all classifications.");
                return new PagedResponseDTO<GetClassificationResponseDTO>();
                 
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

                
                var existing = await _context.TicketClassifications.FirstOrDefaultAsync(x => x.Id == dto.Id && (x.IsSoftDeleted==false || x.IsSoftDeleted ==null));
                if (existing == null)
                {
                    _logger.LogWarning("⚠️ Classification not found for update. Id: {Id}", dto.Id);
                    return new GetClassificationResponseDTO();
                }

                existing.ClassificationName = dto.ClassificationName ?? existing.ClassificationName;
                existing.Description = dto.Description ?? existing.Description;
                existing.IsActive = dto.IsActive ?? existing.IsActive;
                existing.UpdatedById = dto.EmployeeId;
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
        public async Task<bool> DeleteAsync(DeleteClassificationRequestDTO dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid DTO or Id for DeleteClassificationAsync.");
                    return false;
                }
 
                var entity = await _context.TicketClassifications.FirstOrDefaultAsync(x => x.Id == dto.Id && (x.IsSoftDeleted == false || x.IsSoftDeleted == null));
                if (entity == null)
                {
                    _logger.LogWarning("⚠️ Classification not found for deletion. Id: {Id}", dto.Id);
                    return false;
                }

                entity.IsActive = false;
                entity.IsSoftDeleted = true;
                entity.SoftDeletedById = dto.EmployeeId;
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

        Task<GetClassificationResponseDTO?> ITicketClassificationRepository.GetByIdAsync(GetClassificationRequestDTO dTO)
        {
            throw new NotImplementedException();
        }
    }
}
