using AutoMapper;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
       bool isActive)
        {
            return await _context.InsurancePolicies
                .Include(x => x.PolicyType)
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x =>
                    x.Id == insurancePolicyId &&
                    x.TenantId == tenantId &&
                    !x.IsSoftDeleted &&
                    x.IsActive == isActive
                );
        }


        // 🔹 GET LIST (Grid)
        public async Task<PagedResponseDTO<GetInsurancePolicyResponseDTO>> GetListAsync(
         GetInsurancePolicyRequestDTO request)
        {
            // 🔹 Defaults (Handler ke baad safety)
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            int pageSize = request.PageSize > 0 ? request.PageSize : 10;
            string sortOrder = string.IsNullOrWhiteSpace(request.SortOrder)
                ? "desc"
                : request.SortOrder.ToLower();

            // 🔹 IsActive resolve (DTO based)
            bool isActive = request.IsActive ?? true;

            // 🔹 Base query
            var query = _context.InsurancePolicies
                .AsNoTracking()
                .Include(x => x.PolicyType)
                .Include(x => x.Country)
                .Where(x =>
                    x.TenantId == request.Prop.TenantId &&
                    !x.IsSoftDeleted &&
                    x.IsActive == isActive);

            // 🔍 FILTERS (ONLY WHAT DTO HAS)

            if (request.InsurancePolicyId.HasValue)
                query = query.Where(x => x.Id == request.InsurancePolicyId.Value);

            if (request.PolicyTypeId.HasValue)
                query = query.Where(x => x.PolicyTypeId == request.PolicyTypeId.Value);

            if (!string.IsNullOrWhiteSpace(request.InsurancePolicyName))
                query = query.Where(x =>
                    x.InsurancePolicyName.Contains(request.InsurancePolicyName));

            if (!string.IsNullOrWhiteSpace(request.InsurancePolicyNumber))
                query = query.Where(x =>
                    x.InsurancePolicyNumber.Contains(request.InsurancePolicyNumber));

            if (!string.IsNullOrWhiteSpace(request.ProviderName))
                query = query.Where(x =>
                    x.ProviderName!.Contains(request.ProviderName));

            if (request.CountryId.HasValue)
                query = query.Where(x => x.CountryId == request.CountryId.Value);

            if (!string.IsNullOrWhiteSpace(request.AgentName))
                query = query.Where(x =>
                    x.AgentName!.Contains(request.AgentName));

            if (!string.IsNullOrWhiteSpace(request.AgentContactNumber))
                query = query.Where(x =>
                    x.AgentContactNumber!.Contains(request.AgentContactNumber));

            if (request.EmployeeAllowed.HasValue)
                query = query.Where(x => x.EmployeeAllowed == request.EmployeeAllowed.Value);

            if (request.ParentsAllowed.HasValue)
                query = query.Where(x => x.ParentsAllowed == request.ParentsAllowed.Value);

            if (request.InLawsAllowed.HasValue)
                query = query.Where(x => x.InLawsAllowed == request.InLawsAllowed.Value);

            if (request.MaxSpouseAllowed.HasValue)
                query = query.Where(x => x.MaxSpouseAllowed == request.MaxSpouseAllowed.Value);

            if (request.MaxChildAllowed.HasValue)
                query = query.Where(x => x.MaxChildAllowed == request.MaxChildAllowed.Value);

            if (request.StartDate.HasValue)
                query = query.Where(x => x.StartDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(x => x.EndDate <= request.EndDate.Value);

            // 🔃 SORTING
            query = sortOrder == "asc"
                ? query.OrderBy(x => x.AddedDateTime)
                : query.OrderByDescending(x => x.AddedDateTime);

            // 📊 TOTAL COUNT
            int totalCount = await query.CountAsync();

            // 📄 PAGINATION + PROJECTION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetInsurancePolicyResponseDTO
                {
                    InsurancePolicyId = x.Id,

                    PolicyTypeId = x.PolicyTypeId,
                    PolicyTypeName = x.PolicyType != null
                        ? x.PolicyType.PolicyName
                        : string.Empty,

                    InsurancePolicyName = x.InsurancePolicyName,
                    InsurancePolicyNumber = x.InsurancePolicyNumber,
                    ProviderName = x.ProviderName,

                    CountryId = x.CountryId,
                    CountryName = x.Country != null
                        ? x.Country.CountryName
                        : null,

                    StartDate = x.StartDate,
                    EndDate = x.EndDate,

                    AgentName = x.AgentName,
                    AgentContactNumber = x.AgentContactNumber,
                    AgentOfficeNumber = x.AgentOfficeNumber,

                    EmployeeAllowed = x.EmployeeAllowed,
                    MaxSpouseAllowed = x.MaxSpouseAllowed,
                    MaxChildAllowed = x.MaxChildAllowed,
                    ParentsAllowed = x.ParentsAllowed,
                    InLawsAllowed = x.InLawsAllowed,

                    IsActive = x.IsActive,
                    IsSoftDeleted = x.IsSoftDeleted,

                    Remark = x.Remark,
                    Description = x.Description
                })
                .ToListAsync();

            return new PagedResponseDTO<GetInsurancePolicyResponseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }



        // 🔹 UPDATE
        public async Task<bool> UpdateAsync(InsurancePolicy policy)
        {
            try
            {
                _context.InsurancePolicies.Update(policy);

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(
                    dbEx,
                    "DB error while updating InsurancePolicy. PolicyId: {PolicyId}",
                    policy.Id
                );

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating InsurancePolicy. PolicyId: {PolicyId}",
                    policy.Id
                );

                return false;
            }
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
