
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IHolidayRepository
    {
        Task<List<Holiday>> GetTenantHolidaysAsync(long tenantId, long? tenantLocationId, int? year, CancellationToken cancellationToken);
        Task<Holiday?> GetTenantHolidayAsync(long tenantId, long id, CancellationToken cancellationToken);
        Task<Holiday?> GetTenantHolidayForWriteAsync(long tenantId, long id, CancellationToken cancellationToken);
        Task<bool> TenantLocationExistsAsync(long tenantId, long tenantLocationId, CancellationToken cancellationToken);
        Task<Holiday?> FindConflictingHolidayAsync(long tenantId, long tenantLocationId, DateOnly date, long? excludeId, CancellationToken cancellationToken);
        Task<List<Holiday>> GetTenantHolidaysForDuplicateCheckAsync(long tenantId, CancellationToken cancellationToken);
        Task<Holiday> SaveHolidayAsync(Holiday holiday, CancellationToken cancellationToken);
        Task<int> ImportHolidaysAsync(IReadOnlyList<Holiday> holidays, CancellationToken cancellationToken);
        Task<List<Holiday>> GetAllHolidaysAsync();
        Task<IEnumerable<Holiday>> GetHolidaysByTenantAsync(long tenantId, int year);
        Task<IEnumerable<Holiday>> GetHolidaysByCountryAsync(int countryId, int year);
        Task<IEnumerable<Holiday>> GetHolidaysByStateAsync(int countryId, int stateId, int year);
        Task<Holiday?> GetHolidayByIdAsync(long id);
        Task AddHolidayAsync(Holiday holiday);
        Task UpdateHolidayAsync(Holiday holiday);
        Task DeleteHolidayAsync(long id, long SoftDeletedById);
         
    }
}
