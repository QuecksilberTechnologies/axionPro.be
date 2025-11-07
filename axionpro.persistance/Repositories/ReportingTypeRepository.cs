using AutoMapper;
using axionpro.application.DTOs.Manager.ReportingType;
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
    public class ReportingTypeRepository : IReportingTypeRepository
    {
        #region Fields
        private readonly WorkforceDbContext _context;
        private readonly ILogger<ReportingTypeRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<WorkforceDbContext>  _contextFactory;
        #endregion



        #region Constructor
        public ReportingTypeRepository(WorkforceDbContext context, ILogger<ReportingTypeRepository> logger, IMapper mapper, IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }
       
        #endregion

        #region AddAsync
        public async Task<List<GetReportingTypeResponseDTO>> AddAsync(CreateReportingTypeRequestDTO dto)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // Duplicate check
                bool isExist = await context.ReportingTypes
                    .AnyAsync(x => x.TypeName.ToLower() == dto.TypeName.ToLower() && x.IsActive);

                if (isExist)
                    throw new Exception($"Reporting type '{dto.TypeName}' already exists.");

                var entity = _mapper.Map<ReportingType>(dto);
                entity.AddedDateTime = DateTime.UtcNow;

                await context.ReportingTypes.AddAsync(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation("ReportingType '{TypeName}' added successfully by UserId {UserId}", dto.TypeName, dto.AddedById);

                // Return updated list
                var result = await context.ReportingTypes
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.AddedDateTime)
                    .ToListAsync();

                return _mapper.Map<List<GetReportingTypeResponseDTO>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding ReportingType.");
                throw;
            }
        }
        #endregion

        #region AllAsync
        public async Task<List<GetReportingTypeResponseDTO>> AllAsync(GetReportingTypeRequestDTO dto)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var query = context.ReportingTypes.AsQueryable();

                if (!string.IsNullOrWhiteSpace(dto.TypeName))
                    query = query.Where(x => x.TypeName.Contains(dto.TypeName));

                if (dto.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == dto.IsActive.Value);

                var list = await query
                    .OrderByDescending(x => x.AddedDateTime)
                    .ToListAsync();

                return _mapper.Map<List<GetReportingTypeResponseDTO>>(list);
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
                await using var context = await _contextFactory.CreateDbContextAsync();

                var entity = await context.ReportingTypes.FirstOrDefaultAsync(x => x.Id == id);
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
                await using var context = await _contextFactory.CreateDbContextAsync();

                var existing = await context.ReportingTypes.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (existing == null)
                    throw new Exception("Reporting type not found.");

                // Duplicate check (excluding same record)
                bool isDuplicate = await context.ReportingTypes
                    .AnyAsync(x => x.TypeName.ToLower() == dto.TypeName.ToLower() && x.Id != dto.Id);

                if (isDuplicate)
                    throw new Exception($"Reporting type '{dto.TypeName}' already exists.");

                existing.TypeName = dto.TypeName;
                existing.Description = dto.Description;
                existing.IsActive = dto.IsActive;
                existing.UpdatedById = dto.EmployeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                context.ReportingTypes.Update(existing);
                await context.SaveChangesAsync();

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
                await using var context = await _contextFactory.CreateDbContextAsync();

                var existing = await context.ReportingTypes.FirstOrDefaultAsync(x => x.Id == id);
                if (existing == null)
                    throw new Exception("Reporting type not found.");

                existing.IsActive = false;
                existing.UpdatedById = employeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                context.ReportingTypes.Update(existing);
                await context.SaveChangesAsync();

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
