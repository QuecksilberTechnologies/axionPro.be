// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Resets a selected Tenant Employee password through the existing
//           credential repository and password-hashing service.
// ================================================================

using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.ResetPassword;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.ResetPassword.Handlers;

/// <summary>
/// Represents the authorized request to reset a selected Tenant Employee
/// password.
/// </summary>
public sealed class ResetPasswordCommand
    : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Gets the administrator-supplied password reset details.
    /// </summary>
    public ResetEmployeePasswordRequestDTO DTO { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommand"/> class.
    /// </summary>
    /// <param name="dto">The password-reset details.</param>
    public ResetPasswordCommand(ResetEmployeePasswordRequestDTO dto)
    {
        DTO = dto;
    }
}

/// <summary>
/// Handles authorized Tenant Employee password resets using the existing
/// <see cref="IPasswordService"/> and credential repository flow.
/// </summary>
public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommandHandler"/> class.
    /// </summary>
    public ResetPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService,
        IPasswordService passwordService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
        _passwordService = passwordService;
        _logger = logger;
    }

    /// <summary>
    /// Resets the active credential password for an Employee who belongs to
    /// the authenticated Tenant.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (request?.DTO is null ||
            string.IsNullOrWhiteSpace(request.DTO.EmployeeId) ||
            string.IsNullOrWhiteSpace(request.DTO.NewPassword) ||
            string.IsNullOrWhiteSpace(request.DTO.ConfirmPassword))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing);
        }

        if (!string.Equals(
                request.DTO.NewPassword,
                request.DTO.ConfirmPassword,
                StringComparison.Ordinal))
        {
            throw new ValidationErrorException("New password and confirm password do not match.");
        }

        // The MediatR behavior performs the module-code and stored-procedure
        // permission check. This validation supplies the trusted Tenant key
        // required to decode the target Employee identifier.
        var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(
                validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
            request.DTO.EmployeeId,
            validation.Claims.TenantEncriptionKey,
            _idEncoderService);

        if (employeeId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        // Verify Tenant ownership before modifying the existing active
        // LoginCredential selected by the repository.
        var employee = await _unitOfWork.Employees.GetByIdAsync(
            employeeId,
            validation.TenantId,
            true);
        if (employee is null)
        {
            throw new ApiException(
                AppConstants.ErrorMessages.ResourceNotFound,
                StatusCodes.Status404NotFound);
        }

        var passwordHash = _passwordService.HashPassword(request.DTO.NewPassword);
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            _logger.LogError(
                "Password hashing failed while resetting the password for EmployeeId {EmployeeId}.",
                employeeId);
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                StatusCodes.Status500InternalServerError);
        }

        var isUpdated = await _unitOfWork.UserLoginRepository.UpdatePassword(
            employeeId,
            passwordHash.Trim(),
            validation.LoggedInEmployeeId);
        if (!isUpdated)
        {
            throw new ApiException(
                AppConstants.ErrorMessages.ResourceNotFound,
                StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "Reset password completed for EmployeeId {EmployeeId} by EmployeeId {ActorEmployeeId}.",
            employeeId,
            validation.LoggedInEmployeeId);

        return ApiResponse<bool>.Success(
            true,
            "Employee password reset successfully.");
    }
}
