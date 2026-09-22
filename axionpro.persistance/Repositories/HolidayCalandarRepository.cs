using axionpro.application.Interfaces.IRepositories;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using axionpro.domain.Entity;
using axionpro.application.Exceptions;
using Npgsql;



namespace axionpro.persistance.Repositories
{
    public class HolidayCalandarRepository : IHolidayCalandarRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<HolidayCalandarRepository> _logger;

        public HolidayCalandarRepository(WorkforceDbContext context, ILogger<HolidayCalandarRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<OrganizationHolidayCalendar>> GetTenantHolidaysAsync(
            long tenantId,
            long? tenantLocationId,
            int? year,
            CancellationToken cancellationToken)
        {
            var query = _context.OrganizationHolidayCalendars.AsNoTracking()
                .Where(holiday => holiday.TenantId == tenantId
                    && holiday.IsActive == true
                    && holiday.IsSoftDeleted != true);

            if (tenantLocationId.HasValue)
            {
                query = query.Where(holiday => holiday.TenantLocationId == tenantLocationId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(holiday => holiday.HolidayDate.Year == year.Value);
            }

            return query.OrderBy(holiday => holiday.HolidayDate)
                .ThenBy(holiday => holiday.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<OrganizationHolidayCalendar?> GetTenantHolidayAsync(
            long tenantId,
            long id,
            CancellationToken cancellationToken)
        {
            return _context.OrganizationHolidayCalendars.FirstOrDefaultAsync(
                holiday => holiday.Id == id
                    && holiday.TenantId == tenantId
                    && holiday.IsActive == true
                    && holiday.IsSoftDeleted != true,
                cancellationToken);
        }

        public Task<OrganizationHolidayCalendar?> GetTenantHolidayForWriteAsync(
            long tenantId,
            long id,
            CancellationToken cancellationToken)
        {
            return _context.OrganizationHolidayCalendars.FirstOrDefaultAsync(
                holiday => holiday.Id == id
                    && holiday.TenantId == tenantId
                    && holiday.IsSoftDeleted != true,
                cancellationToken);
        }

        public Task<bool> TenantLocationExistsAsync(
            long tenantId,
            long tenantLocationId,
            CancellationToken cancellationToken)
        {
            return _context.TenantLocations.AnyAsync(
                location => location.Id == tenantLocationId
                    && location.TenantId == tenantId
                    && location.IsActive
                    && !location.IsSoftDeleted,
                cancellationToken);
        }

        public Task<OrganizationHolidayCalendar?> FindConflictingHolidayAsync(
            long tenantId,
            long tenantLocationId,
            DateOnly date,
            long? excludeId,
            CancellationToken cancellationToken)
        {
            return _context.OrganizationHolidayCalendars.AsNoTracking().FirstOrDefaultAsync(
                holiday => holiday.TenantId == tenantId
                    && holiday.TenantLocationId == tenantLocationId
                    && holiday.HolidayDate == date
                    && holiday.IsSoftDeleted != true
                    && (!excludeId.HasValue || holiday.Id != excludeId.Value),
                cancellationToken);
        }

        public Task<List<OrganizationHolidayCalendar>> GetTenantHolidaysForDuplicateCheckAsync(
            long tenantId,
            CancellationToken cancellationToken)
        {
            return _context.OrganizationHolidayCalendars.AsNoTracking()
                .Where(holiday => holiday.TenantId == tenantId && holiday.IsSoftDeleted != true)
                .ToListAsync(cancellationToken);
        }

        public async Task<OrganizationHolidayCalendar> SaveHolidayAsync(
            OrganizationHolidayCalendar holiday,
            CancellationToken cancellationToken)
        {
            if (holiday.Id == 0)
            {
                await _context.OrganizationHolidayCalendars.AddAsync(holiday, cancellationToken);
            }

            await SaveChangesWithHolidayConflictAsync(cancellationToken);
            return holiday;
        }

        public async Task<int> ImportHolidaysAsync(
            IReadOnlyList<OrganizationHolidayCalendar> holidays,
            CancellationToken cancellationToken)
        {
            if (holidays.Count == 0)
            {
                return 0;
            }

            await _context.OrganizationHolidayCalendars.AddRangeAsync(holidays, cancellationToken);
            return await SaveChangesWithHolidayConflictAsync(cancellationToken);
        }

        private async Task<int> SaveChangesWithHolidayConflictAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: "UX_OrganizationHolidayCalendar_Tenant_Location_Date_NotDeleted"
                })
            {
                throw new ValidationErrorException(
                    "A holiday entry already exists for this location and date. Edit that entry or soft-delete it before creating another.");
            }
        }

        public async Task<List<OrganizationHolidayCalendar>> GetAllHolidaysAsync()
        {
            try
            {
                return await _context.OrganizationHolidayCalendars
                    .Where(x => x.IsActive==true && !x.IsSoftDeleted == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetAllHolidaysAsync {ex.Message} -  {ex.InnerException} - {ex.StackTrace}");
                return new List<OrganizationHolidayCalendar>();
            }
        }

        public async Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByTenantAsync(long tenantId, int year)
        {
            try
            {
                return await _context.OrganizationHolidayCalendars
                    .Where(x => x.TenantId == tenantId && x.HolidayDate.Year == year && x.IsActive == true && x.IsSoftDeleted != true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetHolidaysByTenantAsync for tenantId: {tenantId}, year: {year}");
                return Enumerable.Empty<OrganizationHolidayCalendar>();
            }
        }

        public async Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByCountryAsync(int countryId, int year)
        {
            try
            {
                return await _context.OrganizationHolidayCalendars
                    .Where(x => x.TenantLocation.CountryId == countryId && x.HolidayDate.Year == year && x.IsActive == true && x.IsSoftDeleted != true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetHolidaysByCountryAsync for countryId: {CountryId}, year: {Year}", countryId, year);
                return Enumerable.Empty<OrganizationHolidayCalendar>();
            }
        }

        public async Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByStateAsync(int countryId, int stateId, int year)
        {
            try
            {
                return await _context.OrganizationHolidayCalendars
                    .Where(x => x.TenantLocation.CountryId == countryId && x.TenantLocation.StateId == stateId && x.HolidayDate.Year == year && x.IsActive == true && x.IsSoftDeleted != true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetHolidaysByStateAsync for countryId: {CountryId}, stateId: {StateId}, year: {Year}", countryId, stateId, year);
                return Enumerable.Empty<OrganizationHolidayCalendar>();
            }
        }

        public async Task<OrganizationHolidayCalendar?> GetHolidayByIdAsync(long id)
        {
            try
            {
                return await _context.OrganizationHolidayCalendars
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true && !x.IsSoftDeleted == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetHolidayByIdAsync for id: {id}");
                return null;
            }
        }

        public async Task AddHolidayAsync(OrganizationHolidayCalendar holiday)
        {
            try
            {
                await _context.OrganizationHolidayCalendars.AddAsync(holiday);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddHolidayAsync");
                throw;
            }
        }

        public async Task UpdateHolidayAsync(OrganizationHolidayCalendar holiday)
        {
            try
            {
                _context.OrganizationHolidayCalendars.Update(holiday);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateHolidayAsync for id: {holiday.Id}");
                throw;
            }
        }

        public async Task DeleteHolidayAsync(long id, long SoftDeletedById)
        {
            try
            {
                var holiday = await _context.OrganizationHolidayCalendars.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true && !x.IsSoftDeleted == true);
                if (holiday != null)
                {
                    holiday.IsSoftDeleted = true;
                    holiday.SoftDeletedById = SoftDeletedById;
                    holiday.DeletedDateTime = DateTime.UtcNow;
                    _context.OrganizationHolidayCalendars.Update(holiday);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DeleteHolidayAsync for id: {id}");
                throw;
            }
        }
    }

}
