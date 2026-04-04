using AutoMapper;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.PolicyTypeDocument;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class PolicyTypeDocumentRepository : IPolicyTypeDocumentRepository
    {
        private readonly WorkforceDbContext _context;
       
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyTypeDocumentRepository> _logger;
        private readonly IEncryptionService _encryptionService;

        public PolicyTypeDocumentRepository(
            WorkforceDbContext context,
            ILogger<PolicyTypeDocumentRepository> logger,
            IMapper mapper,
            IEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            
            _encryptionService = encryptionService;


        }

        // 🔹 CREATE
        public async Task<GetPolicyTypeDocumentResponseDTO> AddAsync(PolicyTypeDocument entity)
        {
            try
            {
               
                await _context.PolicyTypeDocuments.AddAsync(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<GetPolicyTypeDocumentResponseDTO>(entity);

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while inserting CompanyPolicyDocument. PolicyTypeId: {PolicyTypeId}",
                    entity.PolicyTypeId);

                return null;
            }
        }
        public async Task<List<PolicyTypeDocument>> GetByPolicyTypeIdsAsync(
            List<int> policyTypeIds,
    long tenantId)
        {
            try
            {
               
                return await _context.PolicyTypeDocuments
                    .AsNoTracking()
                    .Where(x =>
                        policyTypeIds.Contains(x.PolicyTypeId) &&
                        x.TenantId == tenantId &&
                        x.IsActive == true &&
                        !x.IsSoftDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching CompanyPolicyDocuments for PolicyTypeIds");

                return new List<PolicyTypeDocument>();
            }
        }

        // 🔹 GET BY ID
        public async Task<PolicyTypeDocument?> GetByIdAsync(
            int id,
            long tenantId,
            bool isActive)
        {
            try
            {
               

                return await _context.PolicyTypeDocuments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.PolicyTypeId == id
                        && x.TenantId == tenantId
                        && x.IsActive == isActive
                        && x.IsSoftDeleted!=true);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching CompanyPolicyDocument by Id {Id}",
                    id);

                return null;
            }
        }


        // 🔹 UPDATE
        public async Task<bool> UpdateAsync(PolicyTypeDocument entity)
        {
            try
            {
               
                _context.PolicyTypeDocuments.Update(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while updating CompanyPolicyDocument. Id: {Id}",
                    entity.Id);

                return false;
            }
        }

        // 🔹 SOFT DELETE
        public async Task<bool> SoftDeleteAsync(long id)
        {
            try
            {
               
                var entity = await _context.PolicyTypeDocuments
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsSoftDeleted);

                if (entity == null)
                    return false;

                entity.IsSoftDeleted = true;
                entity.SoftDeletedDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while soft deleting CompanyPolicyDocument. Id: {Id}",
                    id);

                return false;
            }
        }

        // 🔹 EXISTS
        public async Task<bool> ExistsAsync(int policyTypeId, long tenantId)
        {
            try
            {
               
                return await _context.PolicyTypeDocuments.AnyAsync(x =>
                    x.PolicyTypeId == policyTypeId
                    && x.TenantId == tenantId
                    && !x.IsSoftDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while checking existence of CompanyPolicyDocument. PolicyTypeId: {PolicyTypeId}",
                    policyTypeId);

                return false;
            }
        }

        public Task<PagedResponseDTO<GetPolicyTypeDocumentResponseDTO>> GetListAsync(GetPolicyTypeDocumentRequestDTO request)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> SoftDeleteByPolicyTypeIdAsync( int policyTypeId,  long deletedById)
        {
            try
            {
                

                var documents = await _context.PolicyTypeDocuments
                    .Where(x =>
                        x.PolicyTypeId == policyTypeId &&
                        (x.IsSoftDeleted == false || x.IsSoftDeleted == null))
                    .ToListAsync();

                if (!documents.Any())
                    return true; // 👈 no docs is NOT an error

                foreach (var doc in documents)
                {
                    doc.IsSoftDeleted = true;
                    doc.IsActive = false;
                    doc.SoftDeletedById = deletedById;
                    doc.SoftDeletedDateTime = DateTime.UtcNow;
                }
                 
                var rows = await _context.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while soft deleting CompanyPolicyDocuments. PolicyTypeId: {PolicyTypeId}",
                    policyTypeId);

                return false;
            }
        }



        public async Task<bool> SoftDeleteOnlyDocAsync(PolicyTypeDocument entity)
        {        

            _context.PolicyTypeDocuments.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PolicyTypeDocument?> GetPolicyTypeOnlyDocByIdAsync(long id)
        {
            return await _context.PolicyTypeDocuments.FirstOrDefaultAsync(x => x.Id == id && !x.IsSoftDeleted);
        }
    }
}
