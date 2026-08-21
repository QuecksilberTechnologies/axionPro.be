// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Resolves current Tenant employee authorization and composes the lightweight operational-navigation bootstrap response.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.Common.Enums;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.UserAccess;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.UserAccessCmd.Handlers;

#region Query

/// <summary>
/// Represents the authenticated tenant employee's request for their current effective operational navigation.
/// </summary>
public sealed class GetTenantUserAccessQuery
    : IRequest<ApiResponse<TenantUserAccessResponseDTO>>;

#endregion

#region Handler

/// <summary>
/// Resolves the authenticated tenant employee's current effective authorization and composes the lightweight operational-navigation bootstrap response.
/// </summary>
public sealed class GetTenantUserAccessQueryHandler
    : IRequestHandler<GetTenantUserAccessQuery, ApiResponse<TenantUserAccessResponseDTO>>
{
    #region Fields

    private readonly ICommonRequestService _commonRequestService;
    private readonly IUnitOfWork _unitOfWork;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantUserAccessQueryHandler"/> class.
    /// </summary>
    /// <param name="commonRequestService">Validates the authenticated Tenant request.</param>
    /// <param name="unitOfWork">Provides current Tenant role and operational-access queries.</param>
    public GetTenantUserAccessQueryHandler(
        ICommonRequestService commonRequestService,
        IUnitOfWork unitOfWork)
    {
        _commonRequestService = commonRequestService;
        _unitOfWork = unitOfWork;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Retrieves the authenticated tenant employee's current effective module-operation access using the authoritative tenant permission source.
    /// </summary>
    /// <param name="request">The parameterless Tenant access query.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous request.</param>
    /// <returns>The current effective Tenant access bootstrap.</returns>
    public async Task<ApiResponse<TenantUserAccessResponseDTO>> Handle(
        GetTenantUserAccessQuery request,
        CancellationToken cancellationToken)
    {
        // Validate the Tenant identity before resolving current authorization state.
        var validation = await _commonRequestService.ValidateRequestAsync();
        if (!validation.Success ||
            validation.Claims == null ||
            validation.LoggedInEmployeeId <= 0 ||
            validation.TenantId <= 0 ||
            !string.Equals(
                validation.Claims.TokenPurpose,
                ConstantValues.Auth.ToString(),
                StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        // Resolve effective role assignments from the database so runtime changes are reflected.
        var effectiveRoles = await _unitOfWork.UserRoleRepository
            .GetEmployeeRolesWithDetailsByIdAsync(
                validation.LoggedInEmployeeId,
                validation.TenantId,
                cancellationToken);
        var roleIds = effectiveRoles
            .Where(userRole => userRole.RoleId is > 0)
            .Select(userRole => userRole.RoleId!.Value)
            .Distinct()
            .OrderBy(roleId => roleId)
            .ToList();

        // Query the authoritative Tenant permission source using the current effective role set.
        var operationalRows = await _unitOfWork.StoreProcedureRepository
            .GetCurrentTenantOperationalAccessAsync(
                validation.TenantId,
                roleIds,
                cancellationToken);

        var response = new TenantUserAccessResponseDTO
        {
            // Build the operational hierarchy from the employee's current permission union.
            OperationalMenus = BuildOperationalMenus(operationalRows)
        };

        return ApiResponse<TenantUserAccessResponseDTO>.Success(
            response,
            AppConstants.SuccessMessages.TenantUserAccessRetrieved);
    }

    #endregion

    #region Access Composition

    /// <summary>
    /// Builds the legacy MainModule-to-SubModule-to-Module hierarchy while removing duplicate operations granted by multiple roles.
    /// </summary>
    /// <param name="operationalRows">The current permission rows returned by the Tenant operational-access function.</param>
    /// <returns>The ordered Tenant operational navigation hierarchy.</returns>
    private static IReadOnlyCollection<MainModuleDto> BuildOperationalMenus(
        IReadOnlyCollection<RoleModuleOperationResponseDTO> operationalRows)
    {
        return operationalRows
            // Avoid applying a second authorization or hierarchy model that can invalidate valid legacy rows.
            .GroupBy(row => new { row.MainModuleId, row.MainModuleName })
            .OrderBy(group => group.Key.MainModuleId)
            .ThenBy(group => group.Key.MainModuleName, StringComparer.Ordinal)
            .Select(mainGroup => new MainModuleDto
            {
                MainModuleId = mainGroup.Key.MainModuleId,
                MainModuleName = mainGroup.Key.MainModuleName,
                SubModules = mainGroup
                    .GroupBy(row => new { row.ParentModuleId, row.SubModuleName })
                    .OrderBy(group => group.Key.ParentModuleId)
                    .ThenBy(group => group.Key.SubModuleName, StringComparer.Ordinal)
                    .Select(subModuleGroup => new SubModuleDto
                    {
                        SubModuleId = subModuleGroup.Key.ParentModuleId,
                        SubModuleName = subModuleGroup.Key.SubModuleName,
                        Modules = subModuleGroup
                            .GroupBy(row => new
                            {
                                row.ModuleId,
                                row.ModuleName,
                                row.DisplayName,
                                row.ImageIconWeb,
                                row.ImageIconMobile,
                                row.URLPath,
                                row.DataViewStructureId,
                                row.DisplayOn
                            })
                            .OrderBy(group => group.Key.ModuleId)
                            .ThenBy(group => group.Key.ModuleName, StringComparer.Ordinal)
                            .Select(moduleGroup => new ModuleDto
                            {
                                ModuleId = moduleGroup.Key.ModuleId,
                                ModuleName = moduleGroup.Key.ModuleName,
                                DisplayName = moduleGroup.Key.DisplayName,
                                ImageIconWeb = moduleGroup.Key.ImageIconWeb,
                                ImageIconMobile = moduleGroup.Key.ImageIconMobile,
                                SubModuleURL = moduleGroup.Key.URLPath,
                                DataViewStructureId = moduleGroup.Key.DataViewStructureId,
                                DisplayOn = moduleGroup.Key.DisplayOn,
                                Operations = moduleGroup
                                    .GroupBy(row => row.OperationId)
                                    .Select(operationGroup => operationGroup
                                        .OrderBy(row => row.OperationName, StringComparer.Ordinal)
                                        .First())
                                    .OrderBy(row => row.OperationId)
                                    .ThenBy(row => row.OperationName, StringComparer.Ordinal)
                                    .Select(row => new OperationDto
                                    {
                                        OperationId = row.OperationId,
                                        OperationName = row.OperationName
                                    })
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToArray();
    }

    #endregion
}

#endregion
