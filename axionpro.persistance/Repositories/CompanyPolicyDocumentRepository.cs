using AutoMapper;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class CompanyPolicyDocumentRepository : ICompanyPolicyDocumentRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyPolicyDocumentRepository> _logger;
        private readonly IEncryptionService _encryptionService;

        public CompanyPolicyDocumentRepository(
            WorkforceDbContext context,
            ILogger<CompanyPolicyDocumentRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory, IEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
            _encryptionService = encryptionService;


        }

        // 🔹 CREATE
        public async Task<GetCompanyPolicyDocumentResponseDTO> AddAsync(CompanyPolicyDocument entity)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                await context.CompanyPolicyDocuments.AddAsync(entity);
                await context.SaveChangesAsync();

                return _mapper.Map<GetCompanyPolicyDocumentResponseDTO>(entity);

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

        // 🔹 GET BY ID
        public async Task<CompanyPolicyDocument?> GetByIdAsync(
            long id,
            long tenantId,
            bool isActive)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                return await context.CompanyPolicyDocuments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == id
                        && x.TenantId == tenantId
                        && x.IsActive == isActive
                        && !x.IsSoftDeleted);
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

        // 🔹 GET LIST (Grid / PolicyType wise)
        //public async Task<PagedResponseDTO<CompanyPolicyDocumentResponseDTO>> GetListAsync(
        //    GetCompanyPolicyDocumentRequestDTO request)
        //{
        //    try
        //    {
        //        await using var context = await _contextFactory.CreateDbContextAsync();

        //        var query =
        //            from doc in context.CompanyPolicyDocuments
        //            where doc.TenantId == request.Prop.TenantId
        //                  && !doc.IsSoftDeleted
        //            select new CompanyPolicyDocumentResponseDTO
        //            {
        //                Id = doc.Id,
        //                PolicyTypeId = doc.PolicyTypeId,
        //                DocumentTitle = doc.DocumentTitle,
        //                FileName = doc.FileName,
        //                FilePath = doc.FilePath,
        //                IsActive = doc.IsActive,
        //                AddedDateTime = doc.AddedDateTime
        //            };

        //        // 🔍 Filters
        //        if (request.PolicyTypeId.HasValue)
        //            query = query.Where(x => x.PolicyTypeId == request.PolicyTypeId.Value);

        //        if (!string.IsNullOrWhiteSpace(request.DocumentTitle))
        //            query = query.Where(x =>
        //                x.DocumentTitle.Contains(request.DocumentTitle));

        //        if (request.IsActive.HasValue)
        //            query = query.Where(x => x.IsActive == request.IsActive.Value);

        //        var totalCount = await query.CountAsync();

        //        // 🔃 Sorting
        //        query = request.SortOrder?.ToLower() == "asc"
        //            ? query.OrderBy(x => x.AddedDateTime)
        //            : query.OrderByDescending(x => x.AddedDateTime);

        //        // 📄 Paging
        //        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        //        var pageSize = request.PageSize > 0 ? request.PageSize : 10;

        //        var items = await query
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync();

        //        return new PagedResponseDTO<CompanyPolicyDocumentResponseDTO>(
        //            items,
        //            pageNumber,
        //            pageSize,
        //            totalCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(
        //            ex,
        //            "Error while fetching CompanyPolicyDocument list");

        //        return new PagedResponseDTO<CompanyPolicyDocumentResponseDTO>(
        //            new List<CompanyPolicyDocumentResponseDTO>(),
        //            request.PageNumber,
        //            request.PageSize,
        //            0);
        //    }
        //}

        // 🔹 UPDATE
        public async Task<bool> UpdateAsync(CompanyPolicyDocument entity)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                context.CompanyPolicyDocuments.Update(entity);
                await context.SaveChangesAsync();

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
                await using var context = await _contextFactory.CreateDbContextAsync();

                var entity = await context.CompanyPolicyDocuments
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsSoftDeleted);

                if (entity == null)
                    return false;

                entity.IsSoftDeleted = true;
                entity.SoftDeletedDateTime = DateTime.UtcNow;

                await context.SaveChangesAsync();
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
                await using var context = await _contextFactory.CreateDbContextAsync();

                return await context.CompanyPolicyDocuments.AnyAsync(x =>
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

        public Task<PagedResponseDTO<GetCompanyPolicyDocumentResponseDTO>> GetListAsync(GetCompanyPolicyDocumentRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
