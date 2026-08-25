// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates tenant-owned asset statuses from authenticated requests.
// ================================================================

using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.AssetFeatures.Status.Handlers;

#region Command

/// <summary>
/// Represents the request to update an asset status.
/// </summary>
public class UpdateStatusCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatusCommand"/> class.
    /// </summary>
    public UpdateStatusCommand(UpdateStatusRequestDTO dto)
    {
        DTO = dto;
    }

    /// <summary>
    /// Gets the client-supplied update values.
    /// </summary>
    public UpdateStatusRequestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>
/// Handles updates to tenant-owned asset statuses.
/// </summary>
public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICommonRequestService _commonRequestService;
    private readonly ILogger<UpdateStatusCommandHandler> _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatusCommandHandler"/> class.
    /// </summary>
    public UpdateStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICommonRequestService commonRequestService,
        ILogger<UpdateStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _commonRequestService = commonRequestService;
        _logger = logger;
    }

    #endregion

    #region Handle

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(
        UpdateStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.Id <= 0)
        {
            throw new ValidationErrorException(
                "Invalid Status Id.",
                new List<string> { "Status Id must be greater than 0." });
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
                "Invalid Tenant authorization context while updating Asset Status. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
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
                    "Tenant authorization context changed while updating Asset Status. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId, userEmployeeId, tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            case -2:
                _logger.LogWarning(
                    "Invalid Tenant role context while updating Asset Status. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId, userEmployeeId, tokenRoleId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
            case 0:
            default:
                _logger.LogWarning(
                    "Asset Status update permission denied. TenantId: {TenantId}, EmployeeId: {EmployeeId}, ModuleId: {ModuleId}, OperationId: {OperationId}",
                    tenantId, userEmployeeId, request.DTO.ModuleId, request.DTO.OperationId);
                throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        // Load the tenant-owned entity before applying client changes.
        var entity = await _unitOfWork.AssetStatusRepository.GetByIdForTenantAsync(
            request.DTO.Id,
            tenantId,
            cancellationToken);
        if (entity is null)
        {
            throw new ApiException("Asset Status not found or update failed.", 404);
        }

        _mapper.Map(request.DTO, entity);
        entity.UpdatedById = userEmployeeId;
        entity.UpdatedDateTime = DateTime.UtcNow;

        var updated = await _unitOfWork.AssetStatusRepository.UpdateAsync(entity, cancellationToken);
        if (!updated)
        {
            throw new ApiException("Asset Status not found or update failed.", 404);
        }

        return ApiResponse<bool>.Success(true, "Asset Status updated successfully.");
    }

    #endregion
}

#endregion
