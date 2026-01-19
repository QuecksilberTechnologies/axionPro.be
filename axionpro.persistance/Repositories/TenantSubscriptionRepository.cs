using AutoMapper;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Interfaces.IRepositories;
 
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class TenantSubscriptionRepository : ITenantSubscriptionRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantSubscriptionRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public TenantSubscriptionRepository(WorkforceDbContext context, ILogger<TenantSubscriptionRepository> logger, IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }
       
        public async Task<TenantSubscription> AddTenantSubscriptionAsync(TenantSubscription subscription)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();


                await context.TenantSubscriptions.AddAsync(subscription);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ TenantSubscription inserted successfully for TenantId: {TenantId}", subscription.TenantId);

                return subscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while inserting TenantSubscription for TenantId: {TenantId}", subscription.TenantId);
                throw;
            }
        }


        public Task<bool> DeleteTenantSubscriptionAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<TenantSubscription>> GetAllTenantSubscriptionsAsync(TenantSubscription? filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<TenantSubscriptionPlanResponseDTO> GetValidateTenantPlan(TenantSubscriptionPlanRequestDTO dto)
        {
            try
            {
                // ✅ Basic validation
                if (dto == null || dto.TenantId <= 0)
                {
                    _logger?.LogWarning("Invalid DTO passed to GetValidateTenantPlan. DTO is null or TenantId is invalid.");
                    return new TenantSubscriptionPlanResponseDTO();
                }

                _logger?.LogInformation("Validating active tenant plan for TenantId: {TenantId}", dto.TenantId);

                // ✅ Query for active subscription
                var subscription = await _context.TenantSubscriptions
                    .Where(x => x.TenantId == dto.TenantId && x.IsActive)
                    .Select(x => new TenantSubscriptionPlanResponseDTO
                    {
                        Id = x.Id,
                        TenantId = x.TenantId,
                        SubscriptionPlanId = x.SubscriptionPlanId,
                        SubscriptionStartDate = x.SubscriptionStartDate,
                        SubscriptionEndDate = x.SubscriptionEndDate,
                        IsActive = x.IsActive
                    })
                    .FirstOrDefaultAsync();

                if (subscription == null)
                {
                    _logger?.LogWarning("No active subscription found for TenantId: {TenantId}", dto.TenantId);
                    return new TenantSubscriptionPlanResponseDTO();
                    
                }

                _logger?.LogInformation("Active subscription found for TenantId: {TenantId}", dto.TenantId);
                return subscription;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while validating tenant subscription plan for TenantId: {TenantId}", dto?.TenantId);
                return new TenantSubscriptionPlanResponseDTO();

            }
        }



        public async Task<List<TenantSubscriptionPlanResponseDTO>> GetTenantSubscriptionPlanInfoAsync(TenantSubscriptionPlanRequestDTO dto)
    {
        try
        {
                await using var context = await _contextFactory.CreateDbContextAsync();

                if (dto == null)
            {
                _logger.LogWarning("GetTenantSubscriptionPlanInfoAsync called with null DTO.");
                return new List<TenantSubscriptionPlanResponseDTO>();
            }

            _logger.LogInformation("Fetching TenantSubscription records with filters: {@dto}", dto);

            // Base query
            var query = context.TenantSubscriptions.AsQueryable();

            // ✅ Apply filters dynamically
            if (dto.TenantId>0)
                query = query.Where(x => x.TenantId == dto.TenantId);

            if (dto.Id.HasValue && dto.Id > 0)
                query = query.Where(x => x.Id == dto.Id);

            if (dto.SubscriptionPlanId.HasValue && dto.SubscriptionPlanId > 0)
                query = query.Where(x => x.SubscriptionPlanId == dto.SubscriptionPlanId);

            if (dto.StartDate.HasValue)
                query = query.Where(x => x.SubscriptionStartDate >= dto.StartDate.Value);

            if (dto.EndDate.HasValue)
                query = query.Where(x => x.SubscriptionEndDate <= dto.EndDate.Value);

            if (dto.IsActive.HasValue)
                query = query.Where(x => x.IsActive == dto.IsActive.Value);

            if (dto.IsTrial.HasValue)
                query = query.Where(x => x.IsTrial == dto.IsTrial.Value);

            // ✅ Fetch result
            var tenantSubscriptions = await query.ToListAsync();

            if (!tenantSubscriptions.Any())
            {
                _logger.LogInformation("No tenant subscriptions found for given filters.");
                return new List<TenantSubscriptionPlanResponseDTO>();
            }

            // ✅ Map to DTO
            var result = _mapper.Map<List<TenantSubscriptionPlanResponseDTO>>(tenantSubscriptions);

            _logger.LogInformation("Fetched {Count} tenant subscriptions.", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching TenantSubscription records.");
            return new List<TenantSubscriptionPlanResponseDTO>();
        }
    }


    public Task UpdateTenantSubscriptionAsync(TenantSubscription subscription)
        {
            throw new NotImplementedException();
        }
    }
}
