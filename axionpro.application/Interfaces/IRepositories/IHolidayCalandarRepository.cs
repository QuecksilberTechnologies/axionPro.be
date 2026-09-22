
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IHolidayCalandarRepository
    {
        Task<List<OrganizationHolidayCalendar>> GetTenantHolidaysAsync(long tenantId, long? tenantLocationId, int? year, CancellationToken cancellationToken);
        Task<OrganizationHolidayCalendar?> GetTenantHolidayAsync(long tenantId, long id, CancellationToken cancellationToken);
        Task<OrganizationHolidayCalendar?> GetTenantHolidayForWriteAsync(long tenantId, long id, CancellationToken cancellationToken);
        Task<bool> TenantLocationExistsAsync(long tenantId, long tenantLocationId, CancellationToken cancellationToken);
        Task<OrganizationHolidayCalendar?> FindConflictingHolidayAsync(long tenantId, long tenantLocationId, DateOnly date, long? excludeId, CancellationToken cancellationToken);
        Task<List<OrganizationHolidayCalendar>> GetTenantHolidaysForDuplicateCheckAsync(long tenantId, CancellationToken cancellationToken);
        Task<OrganizationHolidayCalendar> SaveHolidayAsync(OrganizationHolidayCalendar holiday, CancellationToken cancellationToken);
        Task<int> ImportHolidaysAsync(IReadOnlyList<OrganizationHolidayCalendar> holidays, CancellationToken cancellationToken);
        Task<List<OrganizationHolidayCalendar>> GetAllHolidaysAsync();
        Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByTenantAsync(long tenantId, int year);
        Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByCountryAsync(int countryId, int year);
        Task<IEnumerable<OrganizationHolidayCalendar>> GetHolidaysByStateAsync(int countryId, int stateId, int year);
        Task<OrganizationHolidayCalendar?> GetHolidayByIdAsync(long id);
        Task AddHolidayAsync(OrganizationHolidayCalendar holiday);
        Task UpdateHolidayAsync(OrganizationHolidayCalendar holiday);
        Task DeleteHolidayAsync(long id, long SoftDeletedById);
         
    }
}
