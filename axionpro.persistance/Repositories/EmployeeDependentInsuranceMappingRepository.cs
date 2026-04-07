using AutoMapper;
using axionpro.application.DTOS.Employee.EnrolledPolicy;
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
        public async Task<List<GetEmployeeEnrolledResponseDTO>> GetByEmployeeIdAsync(long employeeId,long tenantId)
        {
            try
            {
                var data = await _context.EmployeePolicyEnrollment
                    .AsNoTracking()
                    .Where(e =>
                        e.EmployeeId == employeeId &&
                        e.TenantId == tenantId &&
                        e.IsActive == true &&
                        e.IsSoftDeleted !=true && e.IsActive == true)
                    .OrderByDescending(e => e.Id)

                    // 🔥 PROJECTION START
                    .Select(e => new GetEmployeeEnrolledResponseDTO
                    {
                        Id = e.Id,
                        EmployeeId = e.EmployeeId.ToString(), // 🔐 encode later if needed

                        PolicyTypeId = e.PolicyTypeId,
                        InsurancePolicyId = e.InsurancePolicyId,

                        HasDependent = e.HasDependent,

                        StartDate = e.StartDate,
                        EndDate = e.EndDate,

                        // 🔥 DEPENDENTS INCLUDED (NO EXTRA QUERY)
                        Dependents = e.EmployeePolicyDependentMapping
                            .Where(d => d.IsActive == true &&
                                        d.IsSoftDeleted !=true   && d.IsActive == true)
                            .Select(d => new GetEmployeeDependentResponsePolicyDTO
                            {
                                Id = d.Id,
                                DependentId = d.DependentId,
                                Relation = d.RelationType,
                                IsCovered = d.IsCovered
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error fetching EmployeePolicyEnrollment with dependents. EmployeeId: {EmployeeId}, TenantId: {TenantId}",
                    employeeId,
                    tenantId);

                return new List<GetEmployeeEnrolledResponseDTO>();
            }
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

        public async Task<bool> SoftDeleteByEnrollmentIdAsync(List<EmployeePolicyDependentMapping> entities)
        {
            try
            {
                if (entities == null || !entities.Any())
                    return false;

                foreach (var entity in entities)
                {
                    _context.EmployeePolicyDependentMapping.Attach(entity);

                    // 🔥 ONLY REQUIRED FIELDS MARK AS MODIFIED
                    _context.Entry(entity).Property(x => x.IsActive).IsModified = true;
                    _context.Entry(entity).Property(x => x.IsSoftDeleted).IsModified = true;
                    _context.Entry(entity).Property(x => x.SoftDeletedById).IsModified = true;
                    _context.Entry(entity).Property(x => x.DeletedDateTime).IsModified = true;
                }

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in SoftDeleteRangeAsync");
                throw;
            }
        }
        
    }
}
