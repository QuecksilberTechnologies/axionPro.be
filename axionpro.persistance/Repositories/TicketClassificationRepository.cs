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
        public async Task<List<GetClassificationResponseDTO>> AddAsync(AddClassificationRequestDTO dto)
        {
          

            try
            {
                 
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ AddAsync called with null DTO.");

                  return new List<GetClassificationResponseDTO>(); ;
                }

               
                // Duplicate Check
                bool exists = await _context.TicketClassifications
                    .AnyAsync(x => x.ClassificationName.ToLower() == dto.ClassificationName.ToLower()
                                   && x.IsActive
                                   && (x.IsSoftDeleted == null || x.IsSoftDeleted == false) && x.TenantId==dto.TenantId);

                if (exists)
                {
                    _logger.LogWarning("⚠️ Classification with same name already exists: {Name}", dto.ClassificationName);
                    
                    return null;
                }
                List<GetClassificationResponseDTO> result   = new List<GetClassificationResponseDTO>();

                // Map DTO → Entity
                var entity = _mapper.Map<TicketClassification>(dto);
                entity.AddedDateTime = DateTime.UtcNow;
                entity.AddedById = dto.EmployeeId;
                entity.IsActive = dto.IsActive ?? true;
                await _context.TicketClassifications.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation(" Ticket classification added successfully. Id: {Id}", entity.Id);
                var request = new GetClassificationRequestDTO
                {
                    EmployeeId = dto.EmployeeId,
                    RoleId = dto.RoleId,
                    TenantId = dto.TenantId,
              
                };

                var responseDTOs = await GetAllAsync(request);


                return responseDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding ticket classification.");

                return new List<GetClassificationResponseDTO>(); 

            }
        }

        //  GET BY ID
        public async Task<GetClassificationResponseDTO?> GetByIdAsync(GetClassificationRequestDTO dto)
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
                    .FirstOrDefaultAsync(x => x.Id == dto.Id && x.IsActive && (x.IsSoftDeleted == false || x.IsSoftDeleted == null) && x.TenantId == dto.TenantId);

                if (entity == null)
                {
                    _logger.LogWarning("⚠️ No classification found for Id: {Id}", dto.Id);
                    return null;
                }

                var response = _mapper.Map<GetClassificationResponseDTO>(entity);
               
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching classification by Id: {Id}", dto.Id);
                return new GetClassificationResponseDTO();
            }
        }

        //  GET ALL

        public async Task<List<GetClassificationResponseDTO>> GetAllAsync(GetClassificationRequestDTO dto)
        {
            var result = new List<GetClassificationResponseDTO>();
            try
            {
                
                var query = _context.TicketClassifications.AsQueryable();

               
                    query = query.Where(x =>   x.TenantId == dto.TenantId && (x.IsSoftDeleted == false || x.IsSoftDeleted == null));

                var list = await query.AsNoTracking().ToListAsync();

                result = _mapper.Map<List<GetClassificationResponseDTO>>(list);

                _logger.LogInformation(" {Count} classifications fetched successfully.", result.Count);

              

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all classifications.");
                return new List<GetClassificationResponseDTO>();
                 
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
    }
}
