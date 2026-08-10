using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace axionpro.persistance.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly WorkforceDbContext? _context;
        private readonly ILogger? _logger;
        private readonly IMapper? _mapper;
        


        public SubscriptionRepository(WorkforceDbContext context, ILogger<SubscriptionRepository> logger, IMapper mapper)
        {
            _context = context;
            
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<SubscriptionPlan> AddSubscriptionPlanAsync(SubscriptionPlan entity)
        {
            await _context.SubscriptionPlans.AddAsync(entity);

            await _context.SaveChangesAsync();

            return entity;
        }
        public async Task<SubscriptionPlan> UpdateSubscriptionPlanAsync( SubscriptionPlan entity)
        {
            var dbEntity = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (dbEntity == null)
                throw new ApiException("Subscription plan not found.", 404);

            dbEntity.PlanName = entity.PlanName;
            dbEntity.MaxUsers = entity.MaxUsers;
            dbEntity.PerDayPrice = entity.PerDayPrice;
            dbEntity.MonthlyPrice = entity.MonthlyPrice;
            dbEntity.YearlyPrice = entity.YearlyPrice;
            dbEntity.IsMostPopular = entity.IsMostPopular;
            dbEntity.IsCustom = entity.IsCustom;
            dbEntity.CurrencyKey = entity.CurrencyKey;
            dbEntity.IsFree = entity.IsFree;
            dbEntity.IsActive = entity.IsActive;

            _context.SubscriptionPlans.Update(dbEntity);

            await _context.SaveChangesAsync();

            return dbEntity;
        }
       

        public async Task<List<SubscriptionActivePlanDTO>> GetAllPlansAsync()
        {
            try
            {
              //  await using var context = await _contextFactory.CreateDbContext Async();

                var plans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .Select(plan => new SubscriptionActivePlanDTO
                    {
                        Id = plan.Id,
                        PlanName = plan.PlanName,
                        IsMostPopular = plan.IsMostPopular,
                        IsCustom = plan.IsCustom,
                        MaxUsers = plan.MaxUsers,
                        CurrencyKey = plan.CurrencyKey,
                        PerDayPrice = plan.PerDayPrice,
                        MonthlyPrice = plan.MonthlyPrice,
                        YearlyPrice = plan.YearlyPrice,
                        IsFree = plan.IsFree,

                        Modules = plan.PlanModuleMapping
                            .Where(pmm => pmm.IsActive ==true && pmm.Module.IsActive==true)
                            .Select(pmm => new ModuleActiveDTO
                            {
                                Id = pmm.Module.Id,
                                ModuleName = pmm.Module.ModuleName,
                                DisplayName = pmm.Module.DisplayName ?? pmm.Module.ModuleName,
                                ParentModuleId = pmm.Module.ParentModuleId ?? 0,

                                //Operations = pmm.Module.ModuleOperationMappings
                                //    .Where(mop => mop.IsActive == true && mop.Operation.IsActive == true)
                                //    .Select(mop => new OperationActiveDTO
                                //    {
                                //        Id = mop.Id,
                                //        DisplayName = mop.Operation.OperationName
                                //    }).ToList()
                            }).ToList()
                    }).ToListAsync();

                // ✅ Nest modules: Parent -> Child
                foreach (var plan in plans)
                {
                    var moduleDict = plan.Modules.ToDictionary(m => m.Id, m => m);
                    var topLevelModules = new List<ModuleActiveDTO>();

                    foreach (var module in plan.Modules)
                    {
                        if (module.ParentModuleId != 0 && moduleDict.TryGetValue(module.ParentModuleId, out var parent))
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

                _logger.LogInformation("Fetched {Count} subscription plan(s).", plans.Count);

                return plans;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching subscription plans.");
                return new List<SubscriptionActivePlanDTO>();
            }
        }



        public async Task<SubscriptionPlanResponseDTO> GetPlanByIdAsync(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            var result = _mapper.Map< SubscriptionPlanResponseDTO > (plan);

            return result;
        }

    }
    }