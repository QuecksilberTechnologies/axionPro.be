using AutoMapper;
using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using axionpro.domain.Entity;

namespace axionpro.persistance.Repositories
{
    public class PolicyTypeInsuranceMappingRepository : IPolicyTypeInsuranceMappingRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyTypeInsuranceMappingRepository> _logger;
       
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;
        public PolicyTypeInsuranceMappingRepository(WorkforceDbContext context, IMapper mapper, ILogger<PolicyTypeInsuranceMappingRepository> logger,  
            IPasswordService passwordService, IEncryptionService encryptionService)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
            
            _passwordService = passwordService;
            _encryptionService = encryptionService;

        }

        public async Task<GetPolicyTypeInsuranceMappingResponseDTO?> AddAsync( PolicyTypeInsuranceMapping entity)
        {
            try
            {
               
                await _context.PolicyTypeInsuranceMappings.AddAsync(entity);
                await _context.SaveChangesAsync();

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

        public async Task<PolicyTypeInsuranceMapping?> GetByIdAsync(
     int id,
     bool isActive)
        {
            try
            {
               

                return await _context.PolicyTypeInsuranceMappings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.IsActive == isActive &&
                        (x.IsSoftDeleted != true ));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching PolicyTypeInsuranceMapping by Id {Id}",
                    id);

                return null;
            }
        }
        public async Task<PolicyTypeInsuranceMapping?> GetByMappedByInsuranceIdAsync( int id,   bool isActive)
        {
            try
            {
               

                return await _context.PolicyTypeInsuranceMappings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.InsurancePolicyId == id &&
                        x.IsActive == isActive &&
                        (x.IsSoftDeleted != true ));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching PolicyTypeInsuranceMapping by Id {Id}",
                    id);

                return null;
            }
        }

        public async Task<List<GetPolicyTypeInsuranceMappingResponseDTO>> GetMapInsuranceDDLForEmployeeMappingAsync(long tenantId, bool isActive, bool isSoftDeleted)
        {
             
                try
                {
                    var query =
                        from mapping in _context.PolicyTypeInsuranceMappings.AsNoTracking()

                        join pt in _context.PolicyTypes.AsNoTracking()
                            on mapping.PolicyTypeId equals pt.Id

                        join ip in _context.InsurancePolicies.AsNoTracking()
                            on mapping.InsurancePolicyId equals ip.Id

                        where
                            // 🔹 TENANT
                            mapping.TenantId == tenantId &&

                            // 🔥 MAPPING FILTER
                            mapping.IsActive == true &&
                            mapping.IsSoftDeleted != true &&

                            // 🔥 POLICY TYPE FILTER
                            pt.IsActive == true &&
                            pt.IsSoftDelete != true &&

                            // 🔥 INSURANCE FILTER
                            ip.IsActive == true &&
                            ip.IsSoftDeleted != true

                        orderby ip.InsurancePolicyName

                        select new GetPolicyTypeInsuranceMappingResponseDTO
                        {
                            Id = mapping.Id,

                            PolicyTypeId = pt.Id,
                            PolicyName = pt.PolicyName,

                            InsurancePolicyId = ip.Id,
                            InsurancePolicyName = ip.InsurancePolicyName,
                            InsurancePolicyNumber = ip.InsurancePolicyNumber,
                            ProviderName = ip.ProviderName,

                            IsActive = mapping.IsActive
                        };

                    return await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error while fetching Insurance DDL");
                    return new List<GetPolicyTypeInsuranceMappingResponseDTO>();
                }
            }
        public async Task<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>> GetMapInsuranceDetailAsync(int policyId, bool isActive)
        {
            try
            {
                var query =
                    from map in _context.PolicyTypeInsuranceMappings.AsNoTracking()

                    join pt in _context.PolicyTypes.AsNoTracking()
                        on map.PolicyTypeId equals pt.Id

                    join ip in _context.InsurancePolicies.AsNoTracking()
                        on map.InsurancePolicyId equals ip.Id

                    // 🔥 LEFT JOIN DOCUMENT
                    join doc in _context.InsurancePolicyDocuments.AsNoTracking()
                        .Where(d => d.IsActive == true && d.IsSoftDeleted != true)
                        on ip.Id equals doc.InsurancePolicyId into docJoin
                    from doc in docJoin.DefaultIfEmpty()

                    where
                        // 🔹 POLICY FILTER
                        map.PolicyTypeId == policyId &&

                        // 🔹 MAPPING FILTER
                        map.IsActive == isActive &&
                        (map.IsSoftDeleted == false || map.IsSoftDeleted == null) &&

                        // 🔥🔥 FIX: INSURANCE FILTER (MISSING THA)
                        ip.IsActive == true &&
                        (ip.IsSoftDeleted == false || ip.IsSoftDeleted == null)

                    orderby map.Id descending

                    select new GetPolicyTypeInsuranceMapDetailsResponseDTO
                    {
                        Id = map.Id,
                        PolicyTypeId = map.PolicyTypeId,
                        InsuranceTypeId = map.InsurancePolicyId,

                        PolicyName = pt.PolicyName ?? string.Empty,
                        InsuranceTypeName = ip.InsurancePolicyName ?? string.Empty,

                        FileName = doc != null ? doc.FileName : string.Empty,
                        FilePath = doc != null ? doc.FilePath : string.Empty,                      

                        Description = ip.Description,
                        IsActive = map.IsActive,
                        MaxChildAllowed = ip.MaxChildAllowed,
                        MaxSpouseAllowed = ip.MaxSpouseAllowed,
                        InLawsAllowed = ip.InLawsAllowed,
                        ParentsAllowed = ip.ParentsAllowed, 

                    };

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching PolicyType–Insurance mapping. PolicyTypeId: {PolicyTypeId}",
                    policyId);

                return new List<GetPolicyTypeInsuranceMapDetailsResponseDTO>();
            }
        }


        public async Task<PagedResponseDTO<GetPolicyTypeInsuranceMappingResponseDTO>> GetListAsync(
            GetPolicyTypeInsuranceMappingRequestDTO request)
        {
            try
            {
                if (request.Props == null)
                    throw new ArgumentNullException(nameof(request.Props));

                var tenantId = request.Props.TenantId;

                var query =
                    from mapping in _context.PolicyTypeInsuranceMappings.AsNoTracking()

                    join policyType in _context.PolicyTypes.AsNoTracking()
                        on mapping.PolicyTypeId equals policyType.Id

                    // 🔥 LEFT JOIN (Insurance optional)
                    join insurance in _context.InsurancePolicies.AsNoTracking()
                        on mapping.InsurancePolicyId equals insurance.Id
                        into insuranceGroup
                    from insurance in insuranceGroup.DefaultIfEmpty()

                    where
                        // 🔹 TENANT
                        mapping.TenantId == tenantId &&

                        // 🔹 MAPPING FILTER
                        mapping.IsActive == request.IsActive &&
                        (mapping.IsSoftDeleted == false || mapping.IsSoftDeleted == null) &&

                        // 🔥 POLICY TYPE FILTER
                        policyType.IsActive == true &&
                        (policyType.IsSoftDelete == false || policyType.IsSoftDelete == null) &&

                        // 🔥 INSURANCE FILTER (LEFT JOIN SAFE)
                        (
                            insurance == null ||
                            (
                                insurance.IsActive == true &&
                                (insurance.IsSoftDeleted == false || insurance.IsSoftDeleted == null)
                            )
                        )

                    select new GetPolicyTypeInsuranceMappingResponseDTO
                    {
                        Id = mapping.Id,
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

                // ===============================
                // 🔹 OPTIONAL FILTERS
                // ===============================
                if (request.PolicyTypeId.HasValue)
                    query = query.Where(x => x.PolicyTypeId == request.PolicyTypeId.Value);

                if (request.InsurancePolicyId.HasValue)
                    query = query.Where(x => x.InsurancePolicyId == request.InsurancePolicyId.Value);

                if (request.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == request.IsActive.Value);

                // ===============================
                // 🔹 COUNT
                // ===============================
                var totalCount = await query.CountAsync();

                // ===============================
                // 🔹 SORTING
                // ===============================
                var sortOrder = request.SortOrder?.ToLower() ?? "desc";

                query = sortOrder == "asc"
                    ? query.OrderBy(x => x.InsurancePolicyName)
                    : query.OrderByDescending(x => x.InsurancePolicyName);

                // ===============================
                // 🔹 PAGINATION
                // ===============================
                var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
                var pageSize = request.PageSize > 0 ? request.PageSize : 10;

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // ===============================
                // 🔹 RESPONSE
                // ===============================
                return new PagedResponseDTO<GetPolicyTypeInsuranceMappingResponseDTO>(
                    items,
                    pageNumber,
                    pageSize,
                    totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching PolicyTypeInsuranceMapping list");
                throw;
            }
        }

        public async Task<bool> SoftDeleteAsync(PolicyTypeInsuranceMapping entity)
        {
            try
            {

                _context.PolicyTypeInsuranceMappings.Update(entity);
                await _context.SaveChangesAsync();

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

                _context.PolicyTypeInsuranceMappings.Update(entity);
                await _context.SaveChangesAsync();

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
