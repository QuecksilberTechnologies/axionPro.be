// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Soft deletes one Employee identity record.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.IdentitiesInfo.Handlers;

#region Command

public sealed class DeleteIdentityInfoCommand : IRequest<ApiResponse<bool>>
{
    public DeleteIdentityRequestDTO DTO { get; }

    public DeleteIdentityInfoCommand(DeleteIdentityRequestDTO dto)
    {
        DTO = dto;
    }
}

#endregion

#region Handler

public sealed class DeleteIdentityInfoCommandHandler
    : IRequestHandler<DeleteIdentityInfoCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<DeleteIdentityInfoCommandHandler> _logger;

    public DeleteIdentityInfoCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IFileStorageService fileStorageService,
        ILogger<DeleteIdentityInfoCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(
        DeleteIdentityInfoCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _commonRequestService.ValidateTenantUserRequestAsync();

        if (!validation.Success)
            throw new UnauthorizedAccessException(validation.ErrorMessage);

        if (request?.DTO == null || request.DTO.EmployeeIdentityId <= 0)
            throw new ValidationErrorException("Invalid identity id.");

        var identity = await _unitOfWork.EmployeeIdentityRepository
            .GetSingleRecordAsync(request.DTO.EmployeeIdentityId, true)
            ?? throw new ApiException("Identity record not found.", 404);

        if (!await _commonRequestService.CanAccessEmployeeDataAsync(
                validation,
                identity.EmployeeId,
                EmployeeDataAccessRequirement.PersonalDetails,
                cancellationToken))
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);

        if (validation.RoleTypeId != ConstantValues.RoleTypeAdmin &&
            (identity.IsInfoVerified || !identity.IsEditAllowed))
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            identity.IsActive = false;
            identity.IsSoftDeleted = true;
            identity.SoftDeletedById = validation.UserEmployeeId;
            identity.DeletedDateTime = DateTime.UtcNow;

            if (!await _unitOfWork.EmployeeIdentityRepository.SoftDeleteAsync(
                    identity,
                    cancellationToken))
                throw new ApiException("Identity delete failed.", 500);

            await _unitOfWork.CommitTransactionAsync();

            if (!string.IsNullOrWhiteSpace(identity.DocumentFilePath))
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(identity.DocumentFilePath);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Failed to delete identity file after soft delete");
                }
            }

            return ApiResponse<bool>.Success(true, "Identity deleted successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}

#endregion
