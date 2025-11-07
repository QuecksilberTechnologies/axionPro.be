using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    .Where(x => x.TenantId == tenantId && x.HolidayYear == year && x.IsActive== true && !x.IsSoftDeleted == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetHolidaysByTenantAsync for tenantId: {tenantId}, year: {year}");
                return Enumerable.Empty<OrganizationHolidayCalendar>();
            }
        }

        public async Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByCountryAsync(string countryCode, int year)
        {
            try
            {
                return await _context.OrganizationHolidayCalendars
                    .Where(x => x.CountryCode == countryCode && x.HolidayYear == year && x.IsActive == true && !x.IsSoftDeleted == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetHolidaysByCountryAsync for countryCode: {countryCode}, year: {year}");
                return Enumerable.Empty<OrganizationHolidayCalendar>();
            }
        }

        public async Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByStateAsync(string countryCode, string stateCode, int year)
        {
            try
            {
                return await _context.OrganizationHolidayCalendars
                    .Where(x => x.CountryCode == countryCode && x.StateCode == stateCode && x.HolidayYear == year && x.IsActive == true && !x.IsSoftDeleted == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetHolidaysByStateAsync for country: {countryCode}, state: {stateCode}, year: {year}");
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
