using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class PlanModuleMappingRepository : IPlanModuleMappingRepository
    {
        private readonly WorkforceDbContext? _context;
        private readonly ILogger? _logger;



        public PlanModuleMappingRepository(WorkforceDbContext context, ILogger<PlanModuleMappingRepository> logger)
        {
            _context = context;

            _logger = logger;
        }

        public Task AddAsync(PlanModuleMapping planModuleMapping)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<List<Module>> GetAllSubscribedModuleAsync(int? subscriptionPlanId)
        {
            try
            {
                if (subscriptionPlanId == null)
                    return new List<Module>();

                // 🔹 Step 1: Plan ke mapped module IDs nikaal lo
                var mappedModuleIds = await _context.PlanModuleMappings
                    .Where(p => p.SubscriptionPlanId == subscriptionPlanId && p.IsActive==true)
                    .Select(p => p.ModuleId)
                    .ToListAsync();

                if (!mappedModuleIds.Any())
                    return new List<Module>();

                // 🔹 Step 2: In mapped modules ke saare child modules laa lo (recursively)
                var allModules = await _context.Modules
                    .Where(m => m.IsActive)
                    .ToListAsync();

                List<Module> resultModules = new();

                foreach (var id in mappedModuleIds)
                {
                    var parentModule = allModules.FirstOrDefault(m => m.Id == id);
                    if (parentModule != null)
                    {
                        resultModules.Add(parentModule);
                        resultModules.AddRange(GetAllChildModules(allModules, parentModule.Id));
                    }
                }

                // 🔹 Step 3: Duplicate hata do
                resultModules = resultModules.DistinctBy(m => m.Id).ToList();

                return resultModules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching modules for SubscriptionPlanId: {SubscriptionPlanId}", subscriptionPlanId);
                return new List<Module>();
            }
        }

        private List<Module> GetAllChildModules(List<Module> allModules, int parentId)
        {
            var children = allModules.Where(m => m.ParentModuleId == parentId && m.IsActive).ToList();
            List<Module> result = new(children);

            foreach (var child in children)
            {
                result.AddRange(GetAllChildModules(allModules, child.Id));
            }

            return result;
        }

        //public async Task<List<Module>> GetAllSubscribedModuleAsync(int? subscriptionPlanId)
        //{
        //    try
        //    {
        //        if (subscriptionPlanId == null)
        //            return null; // ya return new List<Module>();

        //        var mappings = await _context.PlanModuleMappings
        //            .Where(p => p.SubscriptionPlanId == subscriptionPlanId && p.IsActive == true)
        //            .Include(p => p.Module)
        //            .ToListAsync();

        //        var modules = mappings
        //            .Where(p => p.Module != null)
        //            .Select(p => new Module
        //            {
        //                Id = p.Module.Id,
        //                ModuleName = p.Module.ModuleName,
        //                DisplayName = p.Module.DisplayName ?? string.Empty,
        //                ParentModuleId = p.Module.ParentModuleId,
        //                IsLeafNode = p.Module.IsLeafNode
        //            })
        //            .ToList();

        //        return modules;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching modules for SubscriptionPlanId: {SubscriptionPlanId}", subscriptionPlanId);
        //        return null; // ya return new List<Module>();
        //    }
        //}

        public async Task<PlanModuleMappingResponseDTO> GetModulesBySubscriptionPlanIdAsync(int? subscriptionPlanId)
        {
            try
            {
                if (subscriptionPlanId == null)
                {
                    return new PlanModuleMappingResponseDTO
                    {
                        SubscriptionPlanId = 0,
                        Modules = new List<ModuleWithOperationsDTO>()
                    };
                }

                var mappings = await _context.PlanModuleMappings
                    .Where(p => p.SubscriptionPlanId == subscriptionPlanId && p.IsActive == true)
                    .Include(p => p.Module)
                        .ThenInclude(m => m.ModuleOperationMappings)
                            .ThenInclude(mop => mop.Operation)
                    .Include(p => p.Module)
                        .ThenInclude(m => m.ParentModule) // ✅ Include Parent Module
                    .ToListAsync();

                var response = new PlanModuleMappingResponseDTO
                {
                    SubscriptionPlanId = subscriptionPlanId.Value,
                    Modules = mappings
                        .Where(p => p.Module != null)
                        .Select(p => new ModuleWithOperationsDTO
                        {
                            ModuleId = p.Module.Id,
                            ModuleName = p.Module.ModuleName,
                            DisplayName = p.Module.DisplayName?.ToString()?? string.Empty,
                            ParentModuleId = p.Module.ParentModuleId,
                            MainModuleId = p.Module.ParentModule?.Id ?? null,
                            MainModuleName = p.Module.ParentModule?.ModuleName ?? string.Empty,
                            
                            Operations = p.Module.ModuleOperationMappings
                               .Where(mop =>
                                        mop.IsActive == true &&
                                        mop.Operation != null/* &&
                                        mop.Operation.IsActive == true*/)
                                       .Select(mop => new OperationResponseDTO
                                            {
                                         OperationId = mop.Operation.Id,
                                        
                                          //PageUrl = mop.PageUrl,
                                          //IconUrl = mop.IconUrl
                                           })
                                      .ToList()

                        })
                        .ToList()
                };

                return response;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching modules and operations for SubscriptionPlanId: {SubscriptionPlanId}", subscriptionPlanId);

                return new PlanModuleMappingResponseDTO
                {
                    SubscriptionPlanId = subscriptionPlanId ?? 0,
                    Modules = new List<ModuleWithOperationsDTO>()
                };
            }
        }



        public Task<bool> IsModuleMappedAsync(int subscriptionPlanId, int moduleId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(PlanModuleMapping planModuleMapping)
        {
            throw new NotImplementedException();
        }

        
    }
}
