using AutoMapper;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.ProjectModule;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.DTOS.StoreProcedures.DashboardSummeries;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Module = axionpro.domain.Entity.Module;

namespace axionpro.persistance.Repositories
{
    public class StoreProcedureRepository : IStoreProcedureRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<StoreProcedureRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        

        public StoreProcedureRepository(WorkforceDbContext context, ILogger<StoreProcedureRepository> logger, IMapper mapper, IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory;
            _mapper = mapper;   
        }

        public async Task<List<RoleModuleOperationResponseDTO>> GetActiveRoleModuleOperationsAsync(GetActiveRoleModuleOperationsRequestDTO request)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                string sqlQuery = "EXEC AxionPro.GetActiveRoleModuleOperations @TenantId = {0}, @RoleIds = {1}";

                var result = await context.Set<RoleModuleOperationResponseDTO>()
                    .FromSqlRaw(sqlQuery, request.TenantId, request.RoleIds)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching module permissions for roles: {RoleIds}", request.RoleIds);
                throw;
            }
        }
        public async Task<GetEmployeeCodePatternResponseDTO?> GetTenantEmployeeCodePatternAsync(
      EmployeeCodePatternRequestDTO request)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var result = await context
                    .Set<GetEmployeeCodePatternResponseDTO>()
                    .FromSqlRaw(
                        "EXEC [AxionPro].[GetEmployeeCodePatternByTenant] @TenantId = {0}",
                        request.TenantId
                    )
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error fetching employee code pattern for TenantId: {TenantId}",
                    request.TenantId
                );
                throw;
            }
        }

        public async Task<List<SubscribedModuleResponseDTO>> GetSubscribedModulesByTenantAsync(long tenantId)
        {
            try
            {
               // await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation("Fetching subscribed modules for TenantId: {TenantId}", tenantId);

                var tenantIdParam = new SqlParameter("@TenantId", tenantId);

                string sqlQuery = "EXEC AxionPro.GetSubscribedModuleByTenantId @TenantId";

                var result = await _context.SubscribedModuleResponseDTOs
                    .FromSqlRaw(sqlQuery, tenantIdParam)
                    .ToListAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No modules found for TenantId: {TenantId}", tenantId);
                    return new List<SubscribedModuleResponseDTO>();
                }

                _logger.LogInformation("Total {Count} modules fetched for TenantId: {TenantId}", result.Count, tenantId);
                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred while fetching subscribed modules for TenantId: {TenantId}", tenantId);
                return new List<SubscribedModuleResponseDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching subscribed modules for TenantId: {TenantId}", tenantId);
                return new List<SubscribedModuleResponseDTO>();
            }
        }



        //   The required column 'ParentModuleName' was not present in the results of a 'FromSql' operation.

        //public async Task<List<ProjectSubModuleDetailDTO>> GetDasboardMenuAsync(string Roles)
        //{
        //    try
        //    {
        //       // Roles = "7,5";
        //        // 🔹 Convert List<int> to Comma-Separated String


        //        var sqlQuery = "EXEC AxionPro.GetDashboardMenusForUser @RoleIds, @ErrorMessage OUTPUT";

        //        var roleParam = new SqlParameter("@RoleIds", Roles);
        //        var errorMessageParam = new SqlParameter
        //        {
        //            ParameterName = "@ErrorMessage",
        //            SqlDbType = SqlDbType.NVarChar,
        //            Size = 500,
        //            Direction = ParameterDirection.Output
        //        };

        //        var result = await _context.ProjectSubModuleDetails
        //            .FromSqlRaw(sqlQuery, roleParam, errorMessageParam)
        //            .ToListAsync();

        //        string errorMessage = errorMessageParam.Value?.ToString();
        //        if (!string.IsNullOrEmpty(errorMessage))
        //        {
        //            _logger.LogError("Stored Procedure Error: {ErrorMessage}", errorMessage);
        //            throw new Exception($"Stored Procedure Error: {errorMessage}");
        //        }

        //        // ✅ Parent-Child Relationship Handling
        //        var groupedModules = result
        //            .GroupBy(x => new
        //            {
        //                x.Id,
        //                x.SubModuleName,
        //                x.SubModuleUrl,
        //                x.IsSubModuleDisplayInUi,
        //                x.IsActive,
        //                x.Remark
        //            })
        //            .Select(g => new ProjectSubModuleDetailDTO
        //            {
        //                Id = g.Key.Id,
        //                SubModuleName = g.Key.SubModuleName,
        //                SubModuleURL = g.Key.SubModuleUrl,
        //                IsSubModuleDisplayInUI = g.Key.IsSubModuleDisplayInUi,
        //                IsActive = g.Key.IsActive,
        //                Remark = g.Key.Remark,
        //                ChildPage = g.Select(c => new ProjectChildModuleDetailDTO
        //                {
        //                    Id = c.ProjectChildModuleDetails.FirstOrDefault()?.Id ?? 0,
        //                    ChildModuleName = c.ProjectChildModuleDetails.FirstOrDefault()?.ChildModuleName,
        //                    ChildModuleURL = c.ProjectChildModuleDetails.FirstOrDefault()?.ChildModuleUrl,
        //                    IconImage = c.ProjectChildModuleDetails.FirstOrDefault()?.IconImage,
        //                    IsOperational = c.ProjectChildModuleDetails.FirstOrDefault()?.IsOperational ?? false
        //                }).Where(c => c.Id > 0).ToList()
        //            }).ToList();

        //        return groupedModules;
        //    }
        //    catch (SqlException ex)
        //    {
        //        _logger.LogError(ex, "SQL Error in GetDasboardMenuAsync for RoleIds: {RoleIds}", userRoles);
        //        throw new Exception("Database error occurred while fetching dashboard menu.", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected Error in GetDasboardMenuAsync for RoleIds: {RoleIds}", userRoles);
        //        throw new Exception("An unexpected error occurred while fetching dashboard menu.", ex);
        //    } await _context.Database.ExecuteSqlRawAsync
        //} 





        // Byte Array to Base64 Converter
        public string ConvertToBase64(byte[] iconData)
        {
            if (iconData == null || iconData.Length == 0)
                return string.Empty;

            return Convert.ToBase64String(iconData);
        }


        public async Task<bool> UpdateLoginCredential(LoginRequestDTO loginRequest)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // var ttt = _context.Database.GetDbConnection().ToString();
                _logger.LogInformation("Updating login credential for LoginId: {LoginId}", loginRequest.LoginId);
                // _logger.LogInformation("DBContext", _context.Database.GetDbConnection().ToString());

                string sqlQuery = @"EXEC AxionPro.UpdateLoginCredential 
                            @LoginId, @Latitude, @Longitude, @LoginDevice, 
                            @IpAddressLocal, @IpAddressPublic, @MacAddress, 
                            @Status OUTPUT, @ErrorMessage OUTPUT";

                var statusParam = new SqlParameter("@Status", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var errorMsgParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

                await context.Database.ExecuteSqlRawAsync(sqlQuery,
                    new SqlParameter("@LoginId", loginRequest.LoginId ?? (object)DBNull.Value),
                    new SqlParameter("@Latitude", loginRequest.Latitude),
                    new SqlParameter("@Longitude", loginRequest.Longitude),
                    new SqlParameter("@LoginDevice", loginRequest.LoginDevice),
                    new SqlParameter("@IpAddressLocal", loginRequest.IpAddressLocal ?? (object)DBNull.Value),
                    new SqlParameter("@IpAddressPublic", loginRequest.IpAddressPublic ?? (object)DBNull.Value),
                    new SqlParameter("@MacAddress", loginRequest.MacAddress ?? (object)DBNull.Value),
                    statusParam,
                    errorMsgParam
                );

                if (errorMsgParam.Value != DBNull.Value && !string.IsNullOrEmpty(errorMsgParam.Value.ToString()))
                {
                    _logger.LogError("Update failed for LoginId: {LoginId}, Error: {ErrorMessage}", loginRequest.LoginId, errorMsgParam.Value);
                    return false;
                }

                return true;
                    //(statusParam.Value != DBNull.Value && (int)statusParam.Value == 1) ? "Success" : "No record updated";
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred while updating login credential for LoginId: {LoginId}", loginRequest.LoginId);
              //  return "Database error occurred while updating login credential.";
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating login credential for LoginId: {LoginId}", loginRequest.LoginId);
                return   false; // "An unexpected error occurred.";
            }
        }

        public async Task<long> ValidateActiveUserLoginOnlyAsync(string loginId)
        {
            try
            {
             
                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation("Validating user login for LoginId: {LoginId}", loginId);

                var loginParam = new SqlParameter("@LoginId", loginId ?? (object)DBNull.Value);
                var resultParam = new SqlParameter("@Result", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };

                string sqlQuery = "EXEC AxionPro.ValidateActiveUserLoginOnly @LoginId, @Result OUTPUT";

                await context.Database.ExecuteSqlRawAsync(sqlQuery, loginParam, resultParam);

                return (long)resultParam.Value;  // Output Parameter से Result Return करें
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred while validating user login for LoginId: {LoginId}", loginId);
                return -1;  // Error Case
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating user login for LoginId: {LoginId}", loginId);
                return -1;  // Error Case
            }
        }


        public async Task<EmployeeCountResponseStatsSp?> GetEmployeeCountsAsync(long tenantId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation(
                    "Fetching employee count statistics. TenantId: {TenantId}",
                    tenantId);

                var sql = @"EXEC AxionPro.AllEmployeeCountData @TenantId";
                var tenantParam = new SqlParameter("@TenantId", tenantId);

                var result = context.EmployeeCountResponseStatsSps
                    .FromSqlRaw(sql, tenantParam)
                    .AsNoTracking()
                    .AsEnumerable()     // 🔑 MOST IMPORTANT LINE
                    .FirstOrDefault();

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    "SQL error while fetching employee count statistics. TenantId: {TenantId}",
                    tenantId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while fetching employee count statistics. TenantId: {TenantId}",
                    tenantId);
                throw;
            }
        }



        public async Task<List<GetEmployeeIdentitySp>> GetIdentityRecordAsync(  long employeeId,   int countryId,        bool isActive)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation(
                    "Fetching Employee Identity records. EmployeeId: {EmployeeId}, CountryId: {CountryId}",
                    employeeId, countryId);

                var sql = @"EXEC AxionPro.GetEmployeeIdentityByCountryRule 
                    @EmployeeId,
                    @CountryId,
                    @IsActive";

                var parameters = new[]
                {
            new SqlParameter("@EmployeeId", employeeId),
            new SqlParameter("@CountryId", countryId),
            new SqlParameter("@IsActive", isActive)
                };

                var result = await context.GetEmployeeIdentitySps
                    .FromSqlRaw(sql, parameters)
                    .AsNoTracking()
                    .ToListAsync();

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                    "SQL error while fetching identity records. EmployeeId: {EmployeeId}",
                    employeeId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while fetching identity records. EmployeeId: {EmployeeId}",
                    employeeId);
                throw;
            }
        }

        public async Task<long> ValidateActiveUserCrendentialOnlyAsync(string loginId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                _logger.LogInformation("Validating user login for LoginId: {LoginId}", loginId);

                var loginParam = new SqlParameter("@LoginId", loginId ?? (object)DBNull.Value);
                var resultParam = new SqlParameter("@Result", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };

                string sqlQuery = "EXEC AxionPro.ValidateActiveUserCrendentialOnly @LoginId, @Result OUTPUT";

                await context.Database.ExecuteSqlRawAsync(sqlQuery, loginParam, resultParam);

                return (long)resultParam.Value;  // Output Parameter से Result Return करें
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred while validating user login for LoginId: {LoginId}", loginId);
                return -1;  // Error Case
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating user login for LoginId: {LoginId}");
                return -1;  // Error Case
            }
        }


        
        public async Task<bool> GetHasAccessOperation(GetCheckOperationPermissionRequestDTO checkOperationPermissionRequest)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // SQL query calling the stored procedure and returning the result
                string sqlQuery = @"EXEC AxionPro.CheckPermission 
                            @RoleId, @ProjectChildModuleDetailId, @OperationId, @HasAccess, @IsActive";

                // Parameters definitione
                var parameters = new[]
                {
                   new SqlParameter("@RoleId", checkOperationPermissionRequest.RoleIds),           
                   new SqlParameter("@OperationId", checkOperationPermissionRequest.OperationId),
                   new SqlParameter("@HasAccess", checkOperationPermissionRequest.HasAccess),
                   new SqlParameter("@IsActive", checkOperationPermissionRequest.IsActive)
        };

                // Executing the stored procedure and getting the result (BIT value)
                var result = await context.Database.SqlQueryRaw<bool>(sqlQuery, parameters).ToListAsync();

                // If result is 1 (HasAccess), return true, otherwise false
                if (result != null && result.Count > 0)
                {
                    return result[0];
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle accordingly
                _logger.LogError($"Error in GetHasAccessOperation: {ex.Message}");
                return false;
            }
        }

        public Task<bool> HasPermissionAsync(long userId, string permissionCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTenantValidAsync(long userId, long? tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<int> ValidateUserPasswordAsync(string loginId)
        {
            throw new NotImplementedException();
        }
        public async Task<UpdateTenantEnabledOperationFromModuleOperationResponseDTO> UpdateTenantEnabledOperationFromModuleOperationRequestDTO(UpdateTenantEnabledOperationFromModuleOperationRequestDTO request)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation("Get new Module Operations from ModuleOperation: {TenantId}", request.TenantId);

                var tenantIdParam = new SqlParameter("@TenantId", request.TenantId);

                // SQL query (no need for output param, as your procedure returns scalar)
                string sqlQuery = "DECLARE @Result INT; EXEC @Result = AxionPro.UpdateTenantEnabledOperationFromModuleOperation @TenantId; SELECT @Result";

                var result = await context.Database.ExecuteSqlRawAsync(sqlQuery, tenantIdParam);

                return new UpdateTenantEnabledOperationFromModuleOperationResponseDTO
                {
                    Result = result
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred while updating tenant operation for TenantId: {TenantId}", request.TenantId);

                return new UpdateTenantEnabledOperationFromModuleOperationResponseDTO
                {
                    Result = -1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating tenant operation for TenantId: {TenantId}", request.TenantId);

                return new UpdateTenantEnabledOperationFromModuleOperationResponseDTO
                {
                    Result = -1
                };
            }
        }

        public async Task<List<GetModuleOperationRolePermissionsResponseDTO>> GetTenantModulesConfigurationResponses(GetTenantModuleOperationRolePermissionsRequestDTO request)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var tenantIdParam = new SqlParameter("@TenantId", request.TenantId);

                var flatList = await context.TenantModulesConfigurations
                    .FromSqlRaw("EXEC AxionPro.GetTenantModuleOperationalConfigData @TenantId", tenantIdParam)
                    .ToListAsync();

                var moduleDict = flatList
                    .GroupBy(f => f.ModuleId)
                    .ToDictionary(
                        g => g.Key,
                        g =>
                        {
                            var first = g.First();
                            return new GetModuleOperationRolePermissionsResponseDTO
                            {
                                ModuleId = first.ModuleId,
                                ModuleName = first.ModuleName,
                                IsLeafNode = first.IsLeafNode,
                                IsEnabled = first.IsEnabled,
                                Level = first.Level,
                                BreadCrum = first.BreadCrum,
                                Operations = g
                                    .Where(x => x.OperationId.HasValue)
                                    .Select(x => new OperationDTO_Config                                    {
                                        OperationId = x.OperationId.Value,
                                        OperationName = x.OperationName,
                                        IsOperationUsed = x.IsOperationUsed ?? false
                                    }).ToList(),
                                Children = new List<GetModuleOperationRolePermissionsResponseDTO>()
                            };
                        });

                List<GetModuleOperationRolePermissionsResponseDTO> roots = new();
                foreach (var flat in flatList)
                {
                    if (flat.ParentModuleId.HasValue && moduleDict.ContainsKey(flat.ParentModuleId.Value))
                    {
                        var parent = moduleDict[flat.ParentModuleId.Value];
                        if (!parent.Children.Any(c => c.ModuleId == flat.ModuleId))
                        {
                            parent.Children.Add(moduleDict[flat.ModuleId]);
                        }
                    }
                    else if (!flat.ParentModuleId.HasValue)
                    {
                        if (!roots.Any(r => r.ModuleId == flat.ModuleId))
                        {
                            roots.Add(moduleDict[flat.ModuleId]);
                        }
                    }
                }

                return roots;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tenant module configuration for TenantId: {TenantId}", request.TenantId);
                return new List<GetModuleOperationRolePermissionsResponseDTO>();
            }
        }





        //public async Task<int> ValidateUserLoginAsync(string loginId)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Validating user login for LoginId: {LoginId}", loginId);
        //        var param = new[] { new SqlParameter("@LoginId", loginId ?? (object)DBNull.Value),
        //              new SqlParameter("@result",SqlDbType.Int) { Direction=ParameterDirection.Output} };


        //        string sqlQuery  = @"DECLARE @Result INT;
        //                    EXEC AxionPro.ValidateUserLogin 
        //                        @LoginId = @loginId, 
        //                        @EmployeeId = @EmployeeId OUTPUT;
        //                    SELECT @EmployeeId;";

        //        var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, param);

        //        return result;

        //    }
        //    catch (SqlException ex)
        //    {
        //        _logger.LogError(ex, "SQL Exception occurred while validating user login for LoginId: {LoginId}", loginId);
        //        throw new Exception("Database error occurred while validating user login.");
        //    }

        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while validating user login for LoginId: {LoginId}");
        //        throw;
        //    }
        //}


    }
}
