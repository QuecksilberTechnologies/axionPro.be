// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Authenticates Host users and Tenant employees and creates minimal secure session bootstrap responses.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.Hash;
using axionpro.application.Constants;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Token;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers;

#region Command

/// <summary>
/// Represents a request to authenticate a Host user or Tenant Employee using the existing login request contract.
/// </summary>
public sealed class NewLoginCommand : IRequest<ApiResponse<NewLoginResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewLoginCommand"/> class.
    /// </summary>
    /// <param name="request">The existing client login request.</param>
    public NewLoginCommand(LoginRequestDTO request)
    {
        Request = request;
    }

    /// <summary>
    /// Gets the existing client login request.
    /// </summary>
    public LoginRequestDTO Request { get; }
}

#endregion

#region Handler

/// <summary>
/// Authenticates Host users first, then Tenant Employees, and returns only compact session bootstrap data.
/// </summary>
public sealed class NewLoginCommandHandler : IRequestHandler<NewLoginCommand, ApiResponse<NewLoginResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<NewLoginCommandHandler> _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="NewLoginCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides the existing Host and Tenant authentication repositories.</param>
    /// <param name="passwordService">Verifies the existing credential password hashes.</param>
    /// <param name="tokenService">Generates the existing Tenant access and refresh tokens.</param>
    /// <param name="idEncoderService">Encodes client-facing Tenant and Employee identifiers.</param>
    /// <param name="fileStorageService">Resolves profile-image URLs from stored file keys.</param>
    /// <param name="mapper">Maps the login read model to the compact public user context.</param>
    /// <param name="logger">Records non-sensitive authentication lifecycle information.</param>
    public NewLoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService,
        IIdEncoderService idEncoderService,
        IFileStorageService fileStorageService,
        IMapper mapper,
        ILogger<NewLoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _idEncoderService = idEncoderService;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Authenticates an active Host user or Tenant Employee and creates a compact secure session bootstrap response.
    /// </summary>
    /// <param name="command">The requested Host-user or Tenant Employee login.</param>
    /// <param name="cancellationToken">A token used to cancel authentication processing.</param>
    /// <returns>A successful compact session bootstrap response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the login request is incomplete.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the credential, employee, tenant, subscription, or primary role is invalid.</exception>
    /// <exception cref="ApiException">Thrown when secure token issuance or metadata persistence cannot complete.</exception>
    public async Task<ApiResponse<NewLoginResponseDTO>> Handle(
        NewLoginCommand command,
        CancellationToken cancellationToken)
    {
        #region Authentication

        var request = command?.Request;
        if (request == null ||
            string.IsNullOrWhiteSpace(request.LoginId) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        #region Principal Resolution

        // Preserve legacy precedence: a valid Host login identifier is never evaluated as a Tenant credential.
        var hostUser = await _unitOfWork.HostUserRepository
            .GetByLoginIdAsync(request.LoginId);

        if (hostUser != null)
        {
            return await HandleHostLoginAsync(hostUser, request, cancellationToken);
        }

        #endregion

        // Preserve the legacy stored-function validation before loading the credential required for password verification.
        var validatedEmployeeId = await _unitOfWork.StoreProcedureRepository
            .ValidateActiveUserLoginOnlyAsync(request.LoginId);

        if (validatedEmployeeId < 1)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        // Validate the active tenant login credential before password verification.
        var loginCredential = await _unitOfWork.UserLoginRepository
            .AuthenticateUser(request.LoginId);

        if (loginCredential == null ||
            string.IsNullOrWhiteSpace(loginCredential.Password) ||
            !_passwordService.VerifyPassword(loginCredential.Password, request.Password))
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        // Resolve the minimal employee context required to bootstrap the authenticated session.
        var bootstrap = await _unitOfWork.UserLoginRepository
            .GetNewLoginBootstrapAsync(loginCredential.Id, cancellationToken);

        if (bootstrap == null)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        // The stored-function result and credential-owned employee must identify the same authenticated employee.
        if (bootstrap.EmployeeId != validatedEmployeeId)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var subscription = await _unitOfWork.TenantSubscriptionRepository
            .GetValidateTenantPlan(new TenantSubscriptionPlanRequestDTO { TenantId = bootstrap.TenantId });

        if (subscription == null ||
            !subscription.SubscriptionEndDate.HasValue ||
            subscription.SubscriptionEndDate.Value.Date < DateTime.Today)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Role Resolution

        // Resolve all effective tenant-scoped roles with the same query and filters used by legacy Auth login.
        var effectiveRoleAssignments = await _unitOfWork.UserRoleRepository
            .GetEmployeeRolesWithDetailsByIdAsync(bootstrap.EmployeeId, bootstrap.TenantId);

        var primaryRoleAssignments = effectiveRoleAssignments
            .Where(assignment =>
                assignment.IsPrimaryRole == true &&
                assignment.RoleId.HasValue &&
                assignment.Role != null)
            .ToList();

        // A session must have exactly one trusted primary role; no arbitrary assignment may be selected.
        if (primaryRoleAssignments.Count != 1)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var primaryRoleAssignment = primaryRoleAssignments[0];
        var primaryRole = _mapper.Map<NewLoginRoleDTO>(primaryRoleAssignment.Role!);

        // Return each valid secondary role once, excluding the selected primary role.
        var secondaryRoles = effectiveRoleAssignments
            .Where(assignment =>
                assignment.RoleId.HasValue &&
                assignment.Role != null &&
                assignment.RoleId != primaryRoleAssignment.RoleId)
            .GroupBy(assignment => assignment.RoleId!.Value)
            .Select(assignments => _mapper.Map<NewLoginRoleDTO>(assignments.First().Role!))
            .ToArray();

        #endregion

        #region Session Composition

        var tenantEncryptionKey = await _unitOfWork.TenantEncryptionKeyRepository
            .GetActiveKeyByTenantIdAsync(bootstrap.TenantId, cancellationToken);

        if (tenantEncryptionKey == null || string.IsNullOrWhiteSpace(tenantEncryptionKey.EncryptionKey))
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        var trustedTenantKey = EncryptionSanitizer.SuperSanitize(tenantEncryptionKey.EncryptionKey);
        var accessToken = await _tokenService.GenerateTenantToken(
            CreateTokenInfo(request.LoginId, bootstrap, primaryRole, trustedTenantKey));

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        var accessTokenExpiresAtUtc = _tokenService.GetExpiryFromToken(accessToken);
        if (!accessTokenExpiresAtUtc.HasValue)
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        var refreshToken = await _tokenService.GenerateRefreshToken();
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        // Persist the same refresh-token expiry value that is returned to the client.
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
        var refreshTokenStored = await _unitOfWork.RefreshTokenRepository.InsertAsync(
            new RefreshToken
            {
                LoginId = loginCredential.LoginId,
                UserType = (short)LoginUserType.TenantEmployee,
                LoginCredentialId = loginCredential.Id,
                HostUserId = null,
                Token = HashHelper.Sha256(refreshToken),
                ExpiryDate = refreshTokenExpiresAtUtc,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = request.IpAddressPublic,
                IsRevoked = false
            });

        if (!refreshTokenStored)
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        // Persist only the authenticated request's permitted device and network metadata.
        loginCredential.MacAddress = request.MacAddress;
        loginCredential.IpAddressLocal = request.IpAddressLocal;
        loginCredential.IpAddressPublic = request.IpAddressPublic;
        loginCredential.Latitude = request.Latitude;
        loginCredential.Longitude = request.Longitude;
        loginCredential.LoginDevice = request.LoginDevice;
        loginCredential.UpdatedById = bootstrap.EmployeeId;
        loginCredential.UpdatedDateTime = DateTime.UtcNow;

        if (!await _unitOfWork.UserLoginRepository.UpdateLoginMetadataAsync(loginCredential, cancellationToken))
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        // Build the compact login response without loading navigation or permission data.
        var user = _mapper.Map<NewLoginUserContextDTO>(bootstrap);
        user.EmployeeId = _idEncoderService.EncodeId_long(bootstrap.EmployeeId, trustedTenantKey);
        user.TenantId = _idEncoderService.EncodeId_long(bootstrap.TenantId, trustedTenantKey);
        user.FullName = BuildFullName(bootstrap);
        user.PrimaryRole = primaryRole;
        user.SecondaryRoles = secondaryRoles;

        // Residential geography is intentionally deferred; never infer it from Tenant registration data.
        user.CountryId = null;
        user.CountryName = null;
        user.StateId = null;
        user.StateName = null;
        user.CityId = null;
        user.CityName = null;

        var profileImageKey = await _unitOfWork.Employees.ProfileImage(bootstrap.EmployeeId);
        if (!string.IsNullOrWhiteSpace(profileImageKey))
        {
            user.ProfileImageUrl = _fileStorageService.GetFileUrl(profileImageKey);
        }

        _logger.LogInformation(
            "Issued a compact Tenant Employee session for TenantId {TenantId} and EmployeeId {EmployeeId}.",
            bootstrap.TenantId,
            bootstrap.EmployeeId);

        return ApiResponse<NewLoginResponseDTO>.Success(
            new NewLoginResponseDTO
            {
                UserType = LoginUserType.TenantEmployee.ToString(),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc.Value.ToUniversalTime(),
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
                User = user
            },
            AppConstants.SuccessMessages.LoginSuccessful);

        #endregion
    }

    #endregion

    #region Host Session

    /// <summary>
    /// Authenticates a resolved Host user and creates the compact Host session response.
    /// </summary>
    /// <param name="hostUser">The active Host user selected by the legacy Host-first login precedence.</param>
    /// <param name="request">The existing login request containing the submitted password.</param>
    /// <param name="cancellationToken">A token used to cancel session creation.</param>
    /// <returns>The compact Host session response.</returns>
    private async Task<ApiResponse<NewLoginResponseDTO>> HandleHostLoginAsync(
        HostUser hostUser,
        LoginRequestDTO request,
        CancellationToken cancellationToken)
    {
        #region Host Authentication

        // Recheck the Host state before verifying the existing Host password hash.
        if (!hostUser.IsActive || hostUser.IsSoftDeleted ||
            !_passwordService.VerifyPassword(hostUser.PasswordHash, request.Password))
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var hostRole = await _unitOfWork.HostRoleRepository.GetByIdAsync(hostUser.HostRoleId);
        if (hostRole == null || !hostRole.IsActive || hostRole.IsSoftDeleted)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        #endregion

        #region Host Session Composition

        // Generate Host claims exclusively from the trusted Host principal and Host role.
        var accessToken = await _tokenService.GenerateHostToken(new HostTokenInfoDTO
        {
            HostUserId = hostUser.Id,
            HostRoleId = hostUser.HostRoleId,
            LoginId = hostUser.LoginId,
            Name = hostUser.Name,
            Email = hostUser.Email
        });

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        var accessTokenExpiresAtUtc = _tokenService.GetExpiryFromToken(accessToken);
        if (!accessTokenExpiresAtUtc.HasValue)
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        var refreshToken = await _tokenService.GenerateRefreshToken();
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        // Preserve the Host refresh-token ownership invariant: HostUserId only, never LoginCredentialId.
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
        var refreshTokenStored = await _unitOfWork.RefreshTokenRepository.InsertAsync(
            new RefreshToken
            {
                LoginId = hostUser.LoginId,
                UserType = (short)LoginUserType.Host,
                LoginCredentialId = null,
                HostUserId = hostUser.Id,
                Token = HashHelper.Sha256(refreshToken),
                ExpiryDate = refreshTokenExpiresAtUtc,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ConstantValues.IP,
                IsRevoked = false
            });

        if (!refreshTokenStored)
        {
            throw new ApiException(
                AppConstants.ErrorMessages.InternalServerError,
                (int)HttpStatusCode.InternalServerError);
        }

        // Keep authorization and navigation payloads outside the authentication response boundary.
        return ApiResponse<NewLoginResponseDTO>.Success(
            new NewLoginResponseDTO
            {
                UserType = LoginUserType.Host.ToString(),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc.Value.ToUniversalTime(),
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
                HostUser = new NewLoginHostUserContextDTO
                {
                    HostUserId = hostUser.Id,
                    Name = hostUser.Name,
                    LoginId = hostUser.LoginId,
                    Email = hostUser.Email
                },
                HostRole = new NewLoginHostRoleDTO
                {
                    HostRoleId = hostRole.Id,
                    Name = hostRole.Name
                }
            },
            AppConstants.SuccessMessages.LoginSuccessful);

        #endregion
    }

    #endregion

    #region Session Bootstrap

    /// <summary>
    /// Builds the existing Tenant access-token claim input from trusted server-side login data.
    /// </summary>
    /// <param name="loginId">The authenticated login identifier.</param>
    /// <param name="bootstrap">The validated minimal Tenant Employee bootstrap data.</param>
    /// <param name="primaryRole">The single effective primary role used by the established access-token claim set.</param>
    /// <param name="tenantEncryptionKey">The active tenant key used by the established token and ID-encoding convention.</param>
    /// <returns>The existing Tenant token-service input.</returns>
    private GetTokenInfoDTO CreateTokenInfo(
        string loginId,
        NewLoginBootstrapReadModel bootstrap,
        NewLoginRoleDTO primaryRole,
        string tenantEncryptionKey)
    {
        return new GetTokenInfoDTO
        {
            TenantEncriptionKey = tenantEncryptionKey,
            TenantId = _idEncoderService.EncodeId_long(bootstrap.TenantId, tenantEncryptionKey),
            UserId = loginId,
            EmployeeId = _idEncoderService.EncodeId_long(bootstrap.EmployeeId, tenantEncryptionKey),
            RoleId = primaryRole.RoleId.ToString(),
            RoleTypeId = primaryRole.RoleTypeId.ToString(),
            // Preserve the established token claim source, which uses the primary role's display name.
            RoleTypeName = primaryRole.RoleName,
            EmployeeTypeId = bootstrap.EmployeeTypeId.ToString(),
            EmployeeTypeName = bootstrap.EmployeeTypeName ?? string.Empty,
            GenderId = bootstrap.GenderId.ToString(),
            GenderName = bootstrap.GenderName ?? string.Empty,
            Email = bootstrap.OfficialEmail ?? string.Empty,
            FullName = BuildFullName(bootstrap),
            HasPermanent = bootstrap.HasPermanent,
            TokenPurpose = ConstantValues.Auth.ToString(),
            IssuedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Builds a whitespace-safe employee display name from the minimal bootstrap projection.
    /// </summary>
    /// <param name="bootstrap">The validated minimal Tenant Employee bootstrap data.</param>
    /// <returns>The normalized employee display name.</returns>
    private static string BuildFullName(NewLoginBootstrapReadModel bootstrap)
    {
        return string.Join(
            " ",
            new[] { bootstrap.FirstName, bootstrap.MiddleName, bootstrap.LastName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    #endregion
}

#endregion
