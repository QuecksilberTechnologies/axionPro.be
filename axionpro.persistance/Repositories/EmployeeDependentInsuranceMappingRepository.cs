using AutoMapper;
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
    public class EmployeeDependentInsuranceMappingRepository : IEmployeeDependentInsuranceMappingRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<EmployeeDependentInsuranceMappingRepository> _logger;
        private readonly IMapper _mapper;

        public EmployeeDependentInsuranceMappingRepository(
            WorkforceDbContext context,  IMapper mapper,
            ILogger<EmployeeDependentInsuranceMappingRepository> logger)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;

        }

        // ===============================
        // 🔥 BULK INSERT
        // ===============================
        public async Task<bool> AddRangeAsync(List<EmployeePolicyDependentMapping> entities)
        {
            await _context.EmployeePolicyDependentMapping.AddRangeAsync(entities);
            return await _context.SaveChangesAsync() > 0;
        }

        // ===============================
        // 🔹 GET BY ENROLLMENT
        // ===============================
        public async Task<List<EmployeePolicyDependentMapping>> GetByEnrollmentIdAsync(long enrollmentId, long tenantId)
        {
            return await _context.EmployeePolicyDependentMapping
                .AsNoTracking()
                .Where(x =>
                    x.EmployeePolicyEnrollmentId == enrollmentId &&
                    x.TenantId == tenantId &&
                    x.IsSoftDeleted != true)
                .ToListAsync();
        }

        // ===============================
        // 🔥 SOFT DELETE (REPLACE USE CASE)
        // ===============================
        public async Task<bool> SoftDeleteByEnrollmentIdAsync(long enrollmentId, long tenantId, long userId)
        {
            var list = await _context.EmployeePolicyDependentMapping
                .Where(x =>
                    x.EmployeePolicyEnrollmentId == enrollmentId &&
                    x.TenantId == tenantId &&
                    x.IsSoftDeleted != true)
                .ToListAsync();

            if (!list.Any())
                return true;

            foreach (var item in list)
            {
                item.IsSoftDeleted = true;
                item.IsActive = false;
                item.SoftDeletedById = userId;
                item.DeletedDateTime = DateTime.UtcNow;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
