using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IHolidayCalandarRepository
    {
        Task<List<OrganizationHolidayCalendar>> GetAllHolidaysAsync();
        Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByTenantAsync(long tenantId, int year);
        Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByCountryAsync(string countryCode, int year);
        Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByStateAsync(string countryCode, string stateCode, int year);
        Task<OrganizationHolidayCalendar?> GetHolidayByIdAsync(long id);
        Task AddHolidayAsync(OrganizationHolidayCalendar holiday);
        Task UpdateHolidayAsync(OrganizationHolidayCalendar holiday);
        Task DeleteHolidayAsync(long id, long SoftDeletedById);
         
    }
}
