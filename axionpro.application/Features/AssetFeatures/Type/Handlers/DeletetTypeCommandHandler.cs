// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft-deletes tenant-owned asset types from authenticated requests.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.AssetFeatures.Type.Handlers;

#region Command

/// <summary>
/// Represents the request to delete an asset type.
/// </summary>
public class DeletetTypeCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeletetTypeCommand"/> class.
    /// </summary>
    /// <param name="dto">The asset type selected for deletion.</param>
    public DeletetTypeCommand(DeleteTypeRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the asset type selected for deletion.
    /// </summary>
    public DeleteTypeRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles soft deletion of tenant-owned asset types.
/// </summary>
public class DeletetTypeCommandHandler : IRequestHandler<DeletetTypeCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletetTypeCommandHandler> _logger;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletetTypeCommandHandler"/> class.
    /// </summary>
    public DeletetTypeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeletetTypeCommandHandler> logger,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(
        DeletetTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(
                "Invalid Type Id.",
                new List<string> { "Type Id must be greater than 0." });
        }

        #region Tenant Request Validation

        var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Trusted Request Context

        long userEmployeeId = validation.LoggedInEmployeeId;
        long tenantId = validation.TenantId;
        int tokenRoleId = validation.RoleId;

        if (userEmployeeId <= 0 || tenantId <= 0 || tokenRoleId <= 0)
        {
            _logger.LogWarning(
                "Invalid Tenant authorization context while deleting Asset Type. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                tenantId, userEmployeeId, tokenRoleId);
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Runtime Permission Validation

        var permissionResult = await _unitOfWork.StoreProcedureRepository
            .CheckTenantEmployeePermissionAsync(
                tenantId,
                userEmployeeId,
                tokenRoleId,
                request.DTO.ModuleId,
                request.DTO.OperationId,
                cancellationToken);

        switch (permissionResult.ResultCode)
        {
            case 1:
                break;
            case -1:
                _logger.LogWarning(
                    "Tenant authorization context changed while deleting Asset Type. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId, userEmployeeId, tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            case -2:
                _logger.LogWarning(
                    "Invalid Tenant role context while deleting Asset Type. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId, userEmployeeId, tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            case 0:
            default:
                _logger.LogWarning(
                    "Asset Type deletion permission denied. TenantId: {TenantId}, EmployeeId: {EmployeeId}, ModuleId: {ModuleId}, OperationId: {OperationId}",
                    tenantId, userEmployeeId, request.DTO.ModuleId, request.DTO.OperationId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        var deleted = await _unitOfWork.AssetTypeRepository.DeleteAsync(
            request.DTO.Id,
            tenantId,
            userEmployeeId,
            cancellationToken);
        if (!deleted)
        {
            _logger.LogWarning(
                "Asset Type {AssetTypeId} was not found for tenant {TenantId}.",
                request.DTO.Id,
                tenantId);
            throw new ApiException("Asset Type not found or already deleted.", 404);
        }

        return ApiResponse<bool>.Success(true, "Asset Type deleted successfully.");
    }

    #endregion
}

#endregion
