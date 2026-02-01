using AutoMapper;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
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
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<InsuranceRepository> _logger;

        public InsuranceRepository(
            WorkforceDbContext context,
            IMapper mapper,
            ILogger<InsuranceRepository> logger,
            IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService,
            IEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
        }

        // 🔹 CREATE
        public async Task<GetInsurancePolicyResponseDTO?> AddAsync(InsurancePolicy policy)
        {
            try
            {
                // 1️⃣ Add & Save
                _context.InsurancePolicies.Add(policy);
                await _context.SaveChangesAsync();

                // 2️⃣ JOIN karke full data nikaalo
                var result = await _context.InsurancePolicies
                    .Where(x => x.Id == policy.Id)
                    .Select(x => new GetInsurancePolicyResponseDTO
                    {
                        // Identity
                        InsurancePolicyId = x.Id,
                        TenantId = x.TenantId,

                        // Policy Type
                        PolicyTypeId = x.PolicyTypeId,
                        PolicyTypeName = x.PolicyType.PolicyName,

                        // Policy Info
                        InsurancePolicyName = x.InsurancePolicyName,
                        InsurancePolicyNumber = x.InsurancePolicyNumber,
                        ProviderName = x.ProviderName,

                        // Country
                        CountryId = x.CountryId,
                        CountryName = x.Country != null ? x.Country.CountryName : null,

                        // Dates
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,

                        // Agent
                        AgentName = x.AgentName,
                        AgentContactNumber = x.AgentContactNumber,
                        AgentOfficeNumber = x.AgentOfficeNumber,

                        // Coverage rules
                        EmployeeAllowed = x.EmployeeAllowed,
                        MaxSpouseAllowed = x.MaxSpouseAllowed,
                        MaxChildAllowed = x.MaxChildAllowed,
                        ParentsAllowed = x.ParentsAllowed,
                        InLawsAllowed = x.InLawsAllowed,

                        // Status
                        IsActive = x.IsActive,
                        IsSoftDeleted = x.IsSoftDeleted,

                        // Extra
                        Remark = x.Remark,
                        Description = x.Description,

                        // Audit
                        AddedById = x.AddedById,
                        AddedDateTime = x.AddedDateTime,
                        UpdatedById = x.UpdatedById,
                        UpdatedDateTime = x.UpdatedDateTime,
                        SoftDeletedById = x.SoftDeletedById,
                        DeletedDateTime = x.DeletedDateTime
                    })
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "DB error while creating InsurancePolicy. PolicyName: {PolicyName}",
                    policy.InsurancePolicyName);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while creating InsurancePolicy. PolicyName: {PolicyName}",
                    policy.InsurancePolicyName);

                return null;
            }
        }


        // 🔹 EXISTS (Name + Tenant)
        public async Task<bool> ExistsAsync(
            string insurancePolicyName,
            long tenantId,
            CancellationToken cancellationToken)
        {
            return await _context.InsurancePolicies
                .AnyAsync(x =>
                    x.TenantId == tenantId &&
                    x.InsurancePolicyName == insurancePolicyName &&
                    !x.IsSoftDeleted,
                    cancellationToken);
        }

        // 🔹 GET BY ID
        public async Task<InsurancePolicy?> GetByIdAsync(
            int insurancePolicyId,
            long tenantId,
            CancellationToken cancellationToken)
        {
            return await _context.InsurancePolicies
                .Include(x => x.PolicyType)
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x =>
                    x.Id == insurancePolicyId &&
                    x.TenantId == tenantId &&
                    !x.IsSoftDeleted,
                    cancellationToken);
        }

        // 🔹 GET LIST (Grid)
        public async Task<IReadOnlyList<InsurancePolicy>> GetListAsync(
            long tenantId,
            CancellationToken cancellationToken)
        {
            return await _context.InsurancePolicies
                .Include(x => x.PolicyType)
                .Include(x => x.Country)
                .Where(x =>
                    x.TenantId == tenantId &&
                    !x.IsSoftDeleted)
                .OrderByDescending(x => x.AddedDateTime)
                .ToListAsync(cancellationToken);
        }

        // 🔹 UPDATE
        public Task UpdateAsync(
            InsurancePolicy policy,
            CancellationToken cancellationToken)
        {
            _context.InsurancePolicies.Update(policy);
            return Task.CompletedTask;
        }

        // 🔹 SOFT DELETE
        public async Task SoftDeleteAsync(
            int insurancePolicyId,
            long tenantId,
            long deletedById,
            CancellationToken cancellationToken)
        {
            var policy = await _context.InsurancePolicies
                .FirstOrDefaultAsync(x =>
                    x.Id == insurancePolicyId &&
                    x.TenantId == tenantId &&
                    !x.IsSoftDeleted,
                    cancellationToken);

            if (policy == null)
                return;

            policy.IsSoftDeleted = true;
            policy.SoftDeletedById = deletedById;
            policy.DeletedDateTime = DateTime.UtcNow;

            _context.InsurancePolicies.Update(policy);
        }
    }

}
