using AutoMapper;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class EmployeeManagerMappingRepository: IEmployeeManagerMappingRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeManagerMappingRepository> _logger;
  
        public EmployeeManagerMappingRepository(WorkforceDbContext context, IMapper mapper, ILogger<EmployeeManagerMappingRepository> logger)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;        

        }

        public async Task<bool> AddAsync(EmployeeManagerMapping entity)
        {
            await _context.EmployeeManagerMappings.AddAsync(entity);
            return true;
        }
        public async Task<bool> ExistsPrimaryAsync(long employeeId, long tenantId)
        {
            return await _context.EmployeeManagerMappings.AnyAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.TenantId == tenantId &&
                    x.ReportingTypeId == 1 &&
                    x.IsActive &&
                    (x.IsSoftDeleted != true));
        }

        /// <inheritdoc />
        public Task<bool> IsCurrentDirectReportAsync(
            long tenantId,
            long managerId,
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            if (tenantId <= 0 || managerId <= 0 || employeeId <= 0)
            {
                return Task.FromResult(false);
            }

            var today = DateTime.UtcNow.Date;
            return _context.EmployeeManagerMappings
                .AsNoTracking()
                .AnyAsync(mapping =>
                    mapping.TenantId == tenantId &&
                    mapping.ManagerId == managerId &&
                    mapping.EmployeeId == employeeId &&
                    mapping.IsActive &&
                    mapping.IsSoftDeleted != true &&
                    mapping.EffectiveFrom <= today &&
                    (!mapping.EffectiveTo.HasValue || mapping.EffectiveTo.Value >= today),
                    cancellationToken);
        }
    }
}
