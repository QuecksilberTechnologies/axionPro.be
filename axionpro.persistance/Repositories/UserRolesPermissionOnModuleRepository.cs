using Azure;
using axionpro.application.DTOs.BasicAndRoleBaseMenu;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{

    public class UserRolesPermissionOnModuleRepository : IUserRolesPermissionOnModuleRepository
    {
        private readonly WorkforceDbContext? _context;
        private readonly ILogger<UserRolesPermissionOnModuleRepository>? _logger;

        public UserRolesPermissionOnModuleRepository(WorkforceDbContext? context, ILogger<UserRolesPermissionOnModuleRepository>? logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> AdminAssignModulePermissionAsync(List<RoleModuleAndPermission> insertRoleModulePermissionsRequest)
        {
            try
            {
                if (insertRoleModulePermissionsRequest == null || !insertRoleModulePermissionsRequest.Any())
                {
                    _logger.LogWarning("No role module permissions to assign.");
                    return 0;
                }

                await _context.RoleModuleAndPermissions.AddRangeAsync(insertRoleModulePermissionsRequest);

                // Final save to the database
                return await _context.SaveChangesAsync(); // 👈 returns number of inserted rows
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AdminAssignModulePermissionAsync while assigning permissions.");
                throw;
            }
        }


        public async Task<IEnumerable<UserRolesPermissionOnModuleDTO>> GetModuleListAndOperationByRollIdAsync(List<RoleInfoDTO> roleList, int? forPlatform)
        {
            try
            {
                if (roleList == null || !roleList.Any())
                {
                    _logger?.LogWarning("Role list is empty or null.");
                    return Enumerable.Empty<UserRolesPermissionOnModuleDTO>();
                }
                // Extract Role IDs from the role list
                List<int>? roleIds = roleList.Select(r => r.Id).ToList();
                //var result = await (from rmp in _context.RoleModuleAndPermissions
                //                    join submd in _context.ProjectSubModuleDetails on rmp.SubModuleId equals submd.Id
                //                    join pmd in _context.ProjectModuleDetails on submd.ModuleId equals pmd.Id
                //                    join op in _context.Operations on rmp.OperationId equals op.Id                                    
                //                    select new UserRolesPermissionOnModuleDTO
                //                    {
                //                        Id = rmp.Id, // RoleModuleAndPermission Id
                //                        SubModuleName = submd.SubModuleName, // SubModuleName from SubModule table
                //                        ModuleName = pmd.ModuleName, // ModuleName from Module table
                //                        ModuleDescription = pmd.Remark, // Description from Module table
                //                        ModuleURL = pmd.ModuleUrl, // URL from Module table
                //                       // ImageIcon = rmp.ImageIcon, // Icon from RoleModuleAndPermission
                //                        ActionType = op.OperationName, // OperationName from Operation table
                //                        ActionDescription = op.Remark, // Description from RoleModuleAndPermission
                //                      //  HasAccess = rmp.HasAccess, // Access permission from RoleModuleAndPermission
                //                        IsActive = rmp.IsActive, // Assign directly as it’s non-nullable
                //                        SubModuleDescription = submd.Remark // SubModule description
                //                    }).ToListAsync();




                //    _logger?.LogInformation($"Successfully fetched {result.Count} active records for Role IDs: {string.Join(",", roleIds)}.");

                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while fetching module list and operations by Role ID.");
                throw; // Re-throw the exception after logging
            }
        }
    }

}

