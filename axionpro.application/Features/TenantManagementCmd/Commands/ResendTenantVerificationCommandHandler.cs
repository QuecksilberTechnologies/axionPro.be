// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Resends the existing Tenant onboarding welcome email for an unverified Tenant after Host validation.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Token;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host request to resend onboarding verification for one Tenant.
/// </summary>
public sealed class ResendTenantVerificationCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResendTenantVerificationCommand"/> class.
    /// </summary>
    /// <param name="tenantId">The authoritative Tenant identifier from the route.</param>
    public ResendTenantVerificationCommand(long tenantId)
    {
        TenantId = tenantId;
    }

    /// <summary>
    /// Gets the authoritative Tenant identifier from the route.
    /// </summary>
    public long TenantId { get; }
}

#endregion

#region Handler

/// <summary>
/// Reuses the established Tenant onboarding token and welcome-email infrastructure for an unverified Tenant.
/// </summary>
public sealed class ResendTenantVerificationCommandHandler
    : IRequestHandler<ResendTenantVerificationCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly ITokenService _tokenService;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ResendTenantVerificationCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant and onboarding credential queries.</param>
    /// <param name="commonRequestService">Validates the current Host principal.</param>
    /// <param name="tokenService">Generates the established onboarding token format.</param>
    /// <param name="idEncoderService">Encodes onboarding token identifiers using the existing convention.</param>
    /// <param name="emailService">Sends the established Tenant welcome template.</param>
    /// <param name="configuration">Provides the configured frontend base URL.</param>
    public ResendTenantVerificationCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        ITokenService tokenService,
        IIdEncoderService idEncoderService,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _tokenService = tokenService;
        _idEncoderService = idEncoderService;
        _emailService = emailService;
        _configuration = configuration;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Resends the existing onboarding welcome email only when the Tenant is not verified.
    /// </summary>
    /// <param name="request">The resend-verification command.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>A successful response when the welcome email was sent.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the route identifier is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the Tenant or its legitimate onboarding credential is unavailable.</exception>
    /// <exception cref="ConflictException">Thrown when the Tenant is already verified.</exception>
    public async Task<ApiResponse<bool>> Handle(
        ResendTenantVerificationCommand request,
        CancellationToken cancellationToken)
    {
        await _commonRequestService.ValidateHostUserRequestAsync();

        if (request is null || request.TenantId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantByIdAsync(request.TenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        if (tenant.IsVerified)
        {
            throw new ConflictException(AppConstants.ErrorMessages.TenantAlreadyVerified);
        }

        var onboardingCredential = await _unitOfWork.TenantRepository
            .GetTenantOnboardingCredentialAsync(tenant.Id, cancellationToken);

        if (onboardingCredential?.Employee is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        // Reuse the initial Tenant set-password token format, expiration, URL, and welcome template.
        var tokenInfo = new GetTokenInfoDTO
        {
            EmployeeId = _idEncoderService.EncodeId_long(onboardingCredential.EmployeeId, null),
            TenantId = _idEncoderService.EncodeId_long(tenant.Id, null),
            Email = tenant.TenantEmail,
            FullName = onboardingCredential.Employee.FirstName ?? tenant.ContactPersonName ?? string.Empty,
            TokenPurpose = _idEncoderService.EncodeId_int(ConstantValues.SetPassword, string.Empty),
            IssuedAt = DateTime.UtcNow,
            Expiry = DateTime.UtcNow.AddMinutes(30),
            IsFirstLogin = true,
            ClientType = "Web"
        };

        var token = await _tokenService.GenerateToken(tokenInfo);
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        var baseUrl = _configuration["FrontEndWebURL:BaseUrl"] ?? string.Empty;
        var emailSent = await _emailService.SendTemplatedEmailAsync(
            ConstantValues.WelcomeEmail,
            tenant.TenantEmail,
            tenant.Id,
            new Dictionary<string, string>
            {
                ["UserName"] = tokenInfo.FullName,
                ["VerificationUrl"] = $"{baseUrl}/auth/set-password?token={token}",
                ["LinkExpiryMinutes"] = "30"
            });

        if (!emailSent)
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        return ApiResponse<bool>.Success(
            true,
            AppConstants.SuccessMessages.TenantVerificationResentSuccessfully);
    }

    #endregion
}

#endregion
