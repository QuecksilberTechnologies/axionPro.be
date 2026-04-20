using AutoMapper;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
 
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace axionpro.persistance.Repositories
{
    public class ReportingTypeRepository : IReportingTypeRepository
    {
        #region Fields
        private readonly WorkforceDbContext _context;
        private readonly ILogger<ReportingTypeRepository> _logger;
        private readonly IMapper _mapper;

        #endregion



        #region Constructor
        public ReportingTypeRepository(WorkforceDbContext context, ILogger<ReportingTypeRepository> logger, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            
        }
       
        #endregion

        #region AddAsync
        public async Task<GetReportingTypeResponseDTO> AddAsync(CreateReportingTypeRequestDTO dto )
        {
            try
            {
              
                // Duplicate check
                bool isExist = await _context.ReportingTypes
                    .AnyAsync(x => x.TypeName.ToLower() == dto.TypeName.ToLower() && x.IsActive);

                if (isExist)
                    throw new Exception($"Reporting type '{dto.TypeName}' already exists.");

                var entity = _mapper.Map<ReportingType>(dto);
                entity.AddedDateTime = DateTime.UtcNow;
                entity.AddedById = dto.Prop.EmployeeId;
                entity.SoftDeletedById= null;
                entity.SoftDeletedDateTime= null;
                entity.IsSoftDeleted= false;
                entity.IsActive = dto.IsActive;
                entity.UpdatedById = null;
                entity.UpdatedDateTime = DateTime.UtcNow;
                entity.Description = dto.Description;

                await _context.ReportingTypes.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ReportingType '{TypeName}' added successfully by UserId {UserId}", dto.TypeName, entity.AddedById);

                // Return updated list
                var result = await _context.ReportingTypes
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.AddedDateTime)
                    .ToListAsync();

                return _mapper.Map<GetReportingTypeResponseDTO>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding ReportingType.");
                throw;
            }
        }
        #endregion

        #region AllAsync
        public async Task<PagedResponseDTO<GetReportingTypeResponseDTO>> AllAsync(GetReportingTypeRequestDTO dto)
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
                var query = _context.ReportingTypes
                    .AsNoTracking()
                    .Where(x => x.IsSoftDeleted != true
                                && x.TenantId == dto.Prop.TenantId);

                // ===============================
                // 3️⃣ FILTERS
                // ===============================
                if (!string.IsNullOrWhiteSpace(dto.TypeName))
                    query = query.Where(x => x.TypeName.Contains(dto.TypeName));

                if (dto.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == dto.IsActive.Value);

                // ===============================
                // 4️⃣ TOTAL COUNT
                // ===============================
                var totalCount = await query.CountAsync();

                // ===============================
                // 5️⃣ SORTING
                // ===============================
                query = dto.SortBy?.ToLower() switch
                {
                    "typename" => dto.SortOrder == "asc"
                        ? query.OrderBy(x => x.TypeName)
                        : query.OrderByDescending(x => x.TypeName),

                    _ => query.OrderByDescending(x => x.AddedDateTime)
                };

                // ===============================
                // 6️⃣ PAGINATION + PROJECTION
                // ===============================
                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetReportingTypeResponseDTO
                    {
                        Id = x.Id, // ✅ tumhara rule (encoded id)
                        TypeName = x.TypeName,
                        Description = x.Description,
                        IsActive = x.IsActive,

                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SortBy = dto.SortBy,
                        SortOrder = dto.SortOrder
                    })
                    .ToListAsync();

                // ===============================
                // 7️⃣ RESPONSE
                // ===============================
                return new PagedResponseDTO<GetReportingTypeResponseDTO>
                {
                    Data = data,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasUploadedAll = data.Count >= totalCount,
                   // CompletionPercentage = totalCount == 0 ? 0 : (int)((double)data.Count / totalCount * 100)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ReportingType list.");
                throw;
            }
        }
        #endregion

        #region GetByIdAsync
        public async Task<GetReportingTypeResponseDTO?> GetByIdAsync(long id)
        {
            try
            {
               
                var entity = await _context.ReportingTypes.FirstOrDefaultAsync(x => x.Id == id && x.IsSoftDeleted != true && x.IsActive == true);
                if (entity == null)
                    return null;

                return _mapper.Map<GetReportingTypeResponseDTO>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ReportingType by ID {Id}", id);
                throw;
            }
        }
        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(UpdateReportingTypeRequestDTO dto)
        {
            try
            {
               
                var existing = await _context.ReportingTypes.FirstOrDefaultAsync(x => x.Id == dto.Id && x.IsSoftDeleted!=true && x.IsActive ==true);
                if (existing == null)
                    throw new Exception("Reporting type not found.");

                // Duplicate check (excluding same record)
                bool isDuplicate = await _context.ReportingTypes
                    .AnyAsync(x => x.TypeName.ToLower() == dto.TypeName.ToLower() && x.Id != dto.Id);

                if (isDuplicate)
                    throw new Exception($"Reporting type '{dto.TypeName}' already exists.");

                existing.TypeName = dto.TypeName;
                existing.Description = dto.Description;
                existing.IsActive = dto.IsActive;
                existing.UpdatedById = dto.EmployeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                _context.ReportingTypes.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ReportingType '{TypeName}' updated successfully by UserId {UserId}", dto.TypeName, dto.EmployeeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ReportingType Id {Id}", dto.Id);
                throw;
            }
        }
        #endregion

        #region DeleteAsync
        public async Task<bool> DeleteAsync(long id, long employeeId)
        {
            try
            {
                
                var existing = await _context.ReportingTypes.FirstOrDefaultAsync(x => x.Id == id && x.IsSoftDeleted != true && x.IsActive == true);
                if (existing == null)
                    throw new Exception("Reporting type not found.");

                existing.IsActive = false;
                existing.IsSoftDeleted = true;
                existing.SoftDeletedById = employeeId;
                existing.SoftDeletedDateTime = DateTime.UtcNow;


                _context.ReportingTypes.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ReportingType Id {Id} deactivated by UserId {UserId}", id, employeeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ReportingType Id {Id}", id);
                throw;
            }
        }
        #endregion
    }
}
