using AutoMapper;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;


using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class TenantSubscriptionRepository : ITenantSubscriptionRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<TenantSubscriptionRepository> _logger;
        
        private readonly IMapper _mapper;
       



        public TenantSubscriptionRepository(WorkforceDbContext context, ILogger<TenantSubscriptionRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
           // 
        }

        public async Task<TenantSubscription> AddTenantSubscriptionAsync(TenantSubscription subscription)
        {
            try
            {
                if (subscription == null)
                    throw new ArgumentNullException(nameof(subscription));

                await _context.TenantSubscriptions.AddAsync(subscription);

                _logger.LogInformation(
                    "TenantSubscription added to DbContext successfully for TenantId: {TenantId}",
                    subscription.TenantId);

                return subscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while adding TenantSubscription to DbContext for TenantId: {TenantId}",
                    subscription?.TenantId);
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
                if (dto == null || dto.TenantId <= 0)
                {
                    _logger?.LogWarning("❌ Invalid DTO passed. DTO is null or TenantId <= 0");
                    return new TenantSubscriptionPlanResponseDTO();
                }

                _logger?.LogInformation("🔍 Validating tenant plan for TenantId: {TenantId}", dto.TenantId);

                // 🔎 DB connection state trace
                var connectionState = _context.Database.GetDbConnection().State;
                _logger?.LogInformation("📡 DB Connection State before query: {State}", connectionState);

                // 🔎 Query
                var query = _context.TenantSubscriptions
                    .AsNoTracking()
                    .Where(x => x.TenantId == dto.TenantId && x.IsActive)
                    .Select(x => new TenantSubscriptionPlanResponseDTO
                    {
                        Id = x.Id,
                        TenantId = x.TenantId,
                        SubscriptionPlanId = x.SubscriptionPlanId,
                        SubscriptionStartDate = x.SubscriptionStartDate,
                        SubscriptionEndDate = x.SubscriptionEndDate,
                        IsActive = x.IsActive
                    });

                _logger?.LogInformation("📜 Generated SQL:\n{SQL}", query.ToQueryString());

                var subscription = await query.FirstOrDefaultAsync();

                if (subscription == null)
                {
                    _logger?.LogWarning("⚠ No active subscription found for TenantId: {TenantId}", dto.TenantId);
                    return new TenantSubscriptionPlanResponseDTO();
                }

                _logger?.LogInformation("✅ Active subscription found for TenantId: {TenantId}", dto.TenantId);

                return subscription;
            }
            catch (Exception ex)
            {
                // 🔴 FULL ERROR TRACE
                _logger?.LogError("❌ Exception occurred while validating tenant plan");

                _logger?.LogError("📛 Message: {Message}", ex.Message);

                _logger?.LogError("📛 StackTrace: {StackTrace}", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    _logger?.LogError("📛 InnerException: {Inner}", ex.InnerException.Message);
                }

                // 🔎 DbContext connection state
                try
                {
                    var state = _context.Database.GetDbConnection().State;
                    _logger?.LogError("📡 DB Connection State during error: {State}", state);
                }
                catch { }

                throw; // 🚨 IMPORTANT → don't swallow error
            }
        }



        public async Task<List<TenantSubscriptionPlanResponseDTO>> GetTenantSubscriptionPlanInfoAsync(TenantSubscriptionPlanRequestDTO dto)
    {
        try
        {
                

                if (dto == null)
            {
                _logger.LogWarning("GetTenantSubscriptionPlanInfoAsync called with null DTO.");
                return new List<TenantSubscriptionPlanResponseDTO>();
            }

            _logger.LogInformation("Fetching TenantSubscription records with filters: {@dto}", dto);

            // Base query
            var query = _context.TenantSubscriptions.AsQueryable();

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
