// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Provides persistence and query operations for subscription plans.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories;

/// <summary>
/// Provides entity-based persistence and SQL-projected read operations for Host-managed subscription plans.
/// </summary>
public class SubscriptionRepository : ISubscriptionRepository
{
    #region Fields

    private readonly WorkforceDbContext _context;
    private readonly ILogger<SubscriptionRepository> _logger;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionRepository"/> class.
    /// </summary>
    /// <param name="context">The persistence context for subscription plans.</param>
    /// <param name="logger">The logger used for query diagnostics.</param>
    /// <param name="mapper">The mapper used for the existing plan response projection.</param>
    public SubscriptionRepository(
        WorkforceDbContext context,
        ILogger<SubscriptionRepository> logger,
        IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    #endregion

    #region Create

    /// <summary>
    /// Adds a new subscription plan with server-controlled soft-delete audit defaults.
    /// </summary>
    /// <param name="entity">The plan entity to add.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The added subscription plan entity.</returns>
    public async Task<SubscriptionPlan> AddSubscriptionPlanAsync(
        SubscriptionPlan entity,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Soft-delete audit values are controlled by the server, never the request DTO.
        entity.IsSoftDeleted = false;
        entity.DeletedById = null;
        entity.DeletedDateTime = null;

        await _context.SubscriptionPlans.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    #endregion

    #region Update

    /// <summary>
    /// Persists a tracked subscription plan whose editable fields were prepared by the handler.
    /// </summary>
    /// <param name="entity">The tracked subscription plan entity to update.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The updated subscription plan entity.</returns>
    public async Task<SubscriptionPlan> UpdateSubscriptionPlanAsync(
        SubscriptionPlan entity,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Persist the entity prepared by the handler; delete audit fields remain server-controlled.
        _context.SubscriptionPlans.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    #endregion

    #region Delete

    /// <summary>
    /// Persists an entity whose server-controlled soft-delete audit values were prepared by the handler.
    /// </summary>
    /// <param name="entity">The subscription plan entity to persist.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The soft-deleted subscription plan entity.</returns>
    public async Task<SubscriptionPlan> SoftDeleteSubscriptionPlanAsync(
        SubscriptionPlan entity,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Persist the tracked entity; no physical delete is performed.
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    #endregion

    #region Queries

    /// <summary>
    /// Retrieves non-deleted plans with their module summaries for the requested active status.
    /// </summary>
    /// <param name="isActive">Whether to retrieve active or inactive subscription plans.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The matching subscription plans.</returns>
    public async Task<List<SubscriptionActivePlanDTO>> GetAllPlansAsync(
        bool isActive,
        CancellationToken cancellationToken)
    {
        // Query only non-deleted plans matching the requested active status before materialization.
        var plans = await _context.SubscriptionPlans
            .AsNoTracking()
            .Where(plan => plan.IsActive == isActive && !plan.IsSoftDeleted)
            .Select(plan => new SubscriptionActivePlanDTO
            {
                Id = plan.Id,
                PlanName = plan.PlanName,
                IsActive = plan.IsActive,
                IsMostPopular = plan.IsMostPopular,
                IsCustom = plan.IsCustom,
                MaxUsers = plan.MaxUsers,
                CurrencyKey = plan.CurrencyKey,
                PerDayPrice = plan.PerDayPrice,
                MonthlyPrice = plan.MonthlyPrice,
                YearlyPrice = plan.YearlyPrice,
                IsFree = plan.IsFree,
                Modules = plan.PlanModuleMapping
                    .Where(mapping => mapping.IsActive == true && mapping.Module.IsActive == true)
                    .Select(mapping => new ModuleActiveDTO
                    {
                        Id = mapping.Module.Id,
                        ModuleName = mapping.Module.ModuleName,
                        DisplayName = mapping.Module.DisplayName ?? mapping.Module.ModuleName,
                        ParentModuleId = mapping.Module.ParentModuleId ?? 0
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        // Restore the existing parent-child module structure for each plan response.
        foreach (var plan in plans)
        {
            var moduleDictionary = plan.Modules.ToDictionary(module => module.Id);
            var topLevelModules = new List<ModuleActiveDTO>();

            foreach (var module in plan.Modules)
            {
                if (module.ParentModuleId != 0 &&
                    moduleDictionary.TryGetValue(module.ParentModuleId, out var parent))
                {
                    parent.ChildModules.Add(module);
                }
                else
                {
                    topLevelModules.Add(module);
                }
            }

            plan.Modules = topLevelModules;
        }

        _logger.LogInformation(
            "Fetched {Count} non-deleted subscription plan(s) with IsActive {IsActive}.",
            plans.Count,
            isActive);

        return plans;
    }

    /// <summary>
    /// Retrieves a non-deleted subscription plan response by identifier.
    /// </summary>
    /// <param name="id">The subscription plan identifier.</param>
    /// <returns>The subscription plan response, when found.</returns>
    public async Task<SubscriptionPlanResponseDTO> GetPlanByIdAsync(int id)
    {
        var plan = await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(subscriptionPlan =>
                subscriptionPlan.Id == id && !subscriptionPlan.IsSoftDeleted);

        return _mapper.Map<SubscriptionPlanResponseDTO>(plan);
    }

    #endregion

    #region Validation Queries

    /// <summary>
    /// Retrieves a tracked, non-deleted subscription plan entity for a server-side operation.
    /// </summary>
    /// <param name="id">The subscription plan identifier.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The subscription plan entity, or <see langword="null"/> when it is unavailable.</returns>
    public Task<SubscriptionPlan?> GetNonDeletedSubscriptionPlanByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return _context.SubscriptionPlans
            .FirstOrDefaultAsync(
                subscriptionPlan => subscriptionPlan.Id == id && !subscriptionPlan.IsSoftDeleted,
                cancellationToken);
    }

    /// <summary>
    /// Determines whether the plan is assigned to an active subscription for an active, non-deleted tenant.
    /// </summary>
    /// <param name="subscriptionPlanId">The subscription plan identifier.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns><see langword="true"/> when an active legitimate tenant assignment exists; otherwise, <see langword="false"/>.</returns>
    public Task<bool> IsSubscriptionPlanAssignedToAnyActiveTenantAsync(
        int subscriptionPlanId,
        CancellationToken cancellationToken)
    {
        // Query the authoritative TenantSubscription relationship without loading tenant records.
        return _context.TenantSubscriptions
            .AsNoTracking()
            .AnyAsync(
                subscription =>
                    subscription.SubscriptionPlanId == subscriptionPlanId &&
                    subscription.IsActive &&
                    subscription.Tenant.IsActive &&
                    subscription.Tenant.IsSoftDeleted != true,
                cancellationToken);
    }

    #endregion
}
