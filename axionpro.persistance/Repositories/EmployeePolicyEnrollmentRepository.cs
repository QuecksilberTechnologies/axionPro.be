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
    public class EmployeePolicyEnrollmentRepository : IEmployeePolicyEnrollmentRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeePolicyEnrollmentRepository> _logger;

        public EmployeePolicyEnrollmentRepository(
            WorkforceDbContext context,
            IMapper mapper,
            ILogger<EmployeePolicyEnrollmentRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ===============================
        // 🔹 CREATE
        // ===============================
        public async Task<EmployeePolicyEnrollment> AddAsync(EmployeePolicyEnrollment entity)
        {
            await _context.EmployeePolicyEnrollment.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // ===============================
        // 🔹 GET BY ID
        // ===============================
        public async Task<EmployeePolicyEnrollment?> GetByIdAsync(long id, long tenantId)
        {
            return await _context.EmployeePolicyEnrollment
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.TenantId == tenantId &&
                    x.IsSoftDeleted != true);
        }

        // ===============================
        // 🔹 GET BY EMPLOYEE
        // ===============================
        public async Task<List<EmployeePolicyEnrollment>> GetByEmployeeIdAsync(long employeeId, long tenantId)
        {
            return await _context.EmployeePolicyEnrollment
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.TenantId == tenantId &&
                    x.IsSoftDeleted != true)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        // ===============================
        // 🔹 UPDATE
        // ===============================
        public async Task<bool> UpdateAsync(EmployeePolicyEnrollment entity)
        {
            _context.EmployeePolicyEnrollment.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // ===============================
        // 🔹 SOFT DELETE
        // ===============================
        public async Task<bool> SoftDeleteAsync(EmployeePolicyEnrollment entity)
        {
            entity.IsSoftDeleted = true;
            entity.IsActive = false;
            entity.DeletedDateTime = DateTime.UtcNow;

            _context.EmployeePolicyEnrollment.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
