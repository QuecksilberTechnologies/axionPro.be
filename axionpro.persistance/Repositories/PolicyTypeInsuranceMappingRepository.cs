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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class PolicyTypeInsuranceMappingRepository : IPolicyTypeInsuranceMappingRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyTypeInsuranceMappingRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public PolicyTypeInsuranceMappingRepository(WorkforceDbContext context, IMapper mapper, ILogger<PolicyTypeInsuranceMappingRepository> logger, IDbContextFactory<WorkforceDbContext> contextFactory,
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            _contextFactory = contextFactory;
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        public async Task<GetPolicyTypeInsuranceMappingResponseDTO?> AddAsync( PolicyTypeInsuranceMapping entity)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                await context.PolicyTypeInsuranceMappings.AddAsync(entity);
                await context.SaveChangesAsync();

                // Entity → Response DTO
                var response = _mapper.Map<GetPolicyTypeInsuranceMappingResponseDTO>(entity);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while creating PolicyTypeInsuranceMapping. PolicyTypeId: {PolicyTypeId}, InsurancePolicyId: {InsurancePolicyId}",
                    entity.PolicyTypeId,
                    entity.InsurancePolicyId);

                return null;
            }
        }

        public Task<bool> ExistsAsync(int id, bool isActive)
        {
            throw new NotImplementedException();
        }

        public async Task<PolicyTypeInsuranceMapping?> GetByIdAsync( int id, bool isActive)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.PolicyTypeInsuranceMappings
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id
                    && x.IsActive == isActive
                    && (x.IsSoftDeleted == false || x.IsSoftDeleted == null));
        }


        public async Task<PagedResponseDTO<GetPolicyTypeInsuranceMappingResponseDTO>> GetListAsync(
       GetPolicyTypeInsuranceMappingRequestDTO request)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            if (request.Props == null)
                throw new ArgumentNullException(nameof(request.Props));

            var query =
                from mapping in context.PolicyTypeInsuranceMappings
                join policyType in context.PolicyTypes
                    on mapping.PolicyTypeId equals policyType.Id

                // 🔥 LEFT JOIN (because InsurancePolicyId is nullable)
                join insurance in context.InsurancePolicies
                    on mapping.InsurancePolicyId equals insurance.Id
                    into insuranceGroup
                from insurance in insuranceGroup.DefaultIfEmpty()

                where mapping.TenantId == request.Props.TenantId
                      && (mapping.IsSoftDeleted == false || mapping.IsSoftDeleted == null)

                select new GetPolicyTypeInsuranceMappingResponseDTO
                {
                    PolicyTypeId = mapping.PolicyTypeId,
                    InsurancePolicyId = mapping.InsurancePolicyId,

                    InsurancePolicyName = insurance != null
                        ? insurance.InsurancePolicyName
                        : null,

                    InsurancePolicyNumber = insurance != null
                        ? insurance.InsurancePolicyNumber
                        : null,

                    ProviderName = insurance != null
                        ? insurance.ProviderName
                        : null,

                    PolicyName = policyType.PolicyName,
                    IsActive = mapping.IsActive
                };

            // 🔹 Filters
            if (request.PolicyTypeId.HasValue)
                query = query.Where(x => x.PolicyTypeId == request.PolicyTypeId.Value);

            if (request.InsurancePolicyId.HasValue)
                query = query.Where(x => x.InsurancePolicyId == request.InsurancePolicyId.Value);

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            var totalCount = await query.CountAsync();

            var sortOrder = request.SortOrder?.ToLower() ?? "desc";

            query = sortOrder == "asc"
                ? query.OrderBy(x => x.InsurancePolicyName)
                : query.OrderByDescending(x => x.InsurancePolicyName);

            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponseDTO<GetPolicyTypeInsuranceMappingResponseDTO>(
                items,
                pageNumber,
                pageSize,
                totalCount);
        }

        public async Task<bool> SoftDeleteAsync(PolicyTypeInsuranceMapping entity)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                context.PolicyTypeInsuranceMappings.Update(entity);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating PolicyTypeInsuranceMapping. Id: {Id}",
                    entity.Id);

                return false;
            }
        }

        public async Task<bool> UpdateAsync(PolicyTypeInsuranceMapping entity)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                context.PolicyTypeInsuranceMappings.Update(entity);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating PolicyTypeInsuranceMapping. Id: {Id}",
                    entity.Id);

                return false;
            }
        }

    }
}
