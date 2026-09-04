// ================================================================
// Purpose : Manages Tenant-specific SMTP configuration without exposing SMTP secrets.
// ================================================================

using System.Net.Mail;
using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.Configruations;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantEmailConfigCmd.Handlers;

#region Requests

public sealed class CreateTenantEmailConfigCommand(CreateTenantEmailConfigRequestDTO? dto)
    : IRequest<ApiResponse<TenantEmailConfigResponseDTO>>
{
    public CreateTenantEmailConfigRequestDTO? DTO { get; } = dto;
}

public sealed class UpdateTenantEmailConfigCommand(UpdateTenantEmailConfigRequestDTO? dto)
    : IRequest<ApiResponse<TenantEmailConfigResponseDTO>>
{
    public UpdateTenantEmailConfigRequestDTO? DTO { get; } = dto;
}

public sealed class DeleteTenantEmailConfigCommand(int id, TenantEmailConfigAccessRequestDTO? accessRequest)
    : IRequest<ApiResponse<bool>>
{
    public int Id { get; } = id;
    public TenantEmailConfigAccessRequestDTO? AccessRequest { get; } = accessRequest;
}

public sealed class GetTenantEmailConfigByIdQuery(int id, TenantEmailConfigAccessRequestDTO? accessRequest)
    : IRequest<ApiResponse<TenantEmailConfigResponseDTO>>
{
    public int Id { get; } = id;
    public TenantEmailConfigAccessRequestDTO? AccessRequest { get; } = accessRequest;
}

public sealed class GetAllTenantEmailConfigsQuery(TenantEmailConfigAccessRequestDTO? filter)
    : IRequest<ApiResponse<List<TenantEmailConfigResponseDTO>>>
{
    public TenantEmailConfigAccessRequestDTO? Filter { get; } = filter;
}

#endregion

#region Shared access

public sealed record TenantEmailConfigAccessScope(long TenantId, string TenantEncryptionKey);

/// <summary>Resolves the Tenant boundary from the authenticated Host or Tenant principal.</summary>
public abstract class TenantEmailConfigAccessHandlerBase(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService)
{
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected ICommonRequestService CommonRequestService { get; } = commonRequestService;

    protected async Task<TenantEmailConfigAccessScope> ResolveTenantScopeAsync(
        TenantEmailConfigAccessRequestDTO? accessRequest,
        CancellationToken cancellationToken)
    {
        if (accessRequest is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var principal = await CommonRequestService.ValidateAuthenticatedRequestAsync();
        return principal.UserType switch
        {
            LoginUserType.Host => await ResolveHostScopeAsync(accessRequest, cancellationToken),
            LoginUserType.TenantEmployee => await ResolveTenantEmployeeScopeAsync(cancellationToken),
            _ => throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized)
        };
    }

    protected TenantEmailConfigResponseDTO MapResponse(
        IMapper mapper,
        TenantEmailConfig configuration,
        TenantEmailConfigAccessScope scope)
    {
        var response = mapper.Map<TenantEmailConfigResponseDTO>(configuration);
        response.TenantId = HostTenantIdentifierProtector.Encrypt(
            configuration.TenantId,
            scope.TenantEncryptionKey,
            idEncoderService);
        return response;
    }

    private async Task<TenantEmailConfigAccessScope> ResolveHostScopeAsync(
        TenantEmailConfigAccessRequestDTO accessRequest,
        CancellationToken cancellationToken)
    {
        var hostContext = await CommonRequestService.ValidateHostUserPermissionRequestAsync();
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            accessRequest.TenantId,
            hostContext.TenantEncryptionKey,
            idEncoderService);

        if (await UnitOfWork.TenantRepository.GetHostManagedTenantByIdAsync(tenantId, cancellationToken) is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        return new TenantEmailConfigAccessScope(tenantId, hostContext.TenantEncryptionKey);
    }

    private async Task<TenantEmailConfigAccessScope> ResolveTenantEmployeeScopeAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tenantContext = await CommonRequestService.ValidateTenantUserRequestAsync();
        if (!tenantContext.Success ||
            tenantContext.TenantId <= 0 ||
            string.IsNullOrWhiteSpace(tenantContext.Claims?.TenantEncriptionKey))
        {
            throw new UnauthorizedAccessException(
                tenantContext.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }

        return new TenantEmailConfigAccessScope(
            tenantContext.TenantId,
            tenantContext.Claims.TenantEncriptionKey);
    }
}

#endregion

#region Handlers

public sealed class CreateTenantEmailConfigCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IMapper mapper,
    ILogger<CreateTenantEmailConfigCommandHandler> logger)
    : TenantEmailConfigAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService),
        IRequestHandler<CreateTenantEmailConfigCommand, ApiResponse<TenantEmailConfigResponseDTO>>
{
    public async Task<ApiResponse<TenantEmailConfigResponseDTO>> Handle(
        CreateTenantEmailConfigCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveTenantScopeAsync(dto, cancellationToken);
        var values = TenantEmailConfigInput.Normalize(dto);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (values.IsActive)
            {
                await UnitOfWork.TenantEmailConfigRepository
                    .DeactivateOtherActiveAsync(scope.TenantId, null, cancellationToken);
            }

            var configuration = values.ToEntity(scope.TenantId);
            await UnitOfWork.TenantEmailConfigRepository.AddAsync(configuration, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Created Tenant SMTP configuration {TenantEmailConfigId} for Tenant {TenantId}.", configuration.Id, scope.TenantId);
            return ApiResponse<TenantEmailConfigResponseDTO>.Success(
                MapResponse(mapper, configuration, scope),
                AppConstants.SuccessMessages.TenantEmailConfigCreated);
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

public sealed class UpdateTenantEmailConfigCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IMapper mapper,
    ILogger<UpdateTenantEmailConfigCommandHandler> logger)
    : TenantEmailConfigAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService),
        IRequestHandler<UpdateTenantEmailConfigCommand, ApiResponse<TenantEmailConfigResponseDTO>>
{
    public async Task<ApiResponse<TenantEmailConfigResponseDTO>> Handle(
        UpdateTenantEmailConfigCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        if (dto.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var scope = await ResolveTenantScopeAsync(dto, cancellationToken);
        var configuration = await UnitOfWork.TenantEmailConfigRepository
            .GetForUpdateAsync(scope.TenantId, dto.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantEmailConfigNotFound);
        var values = TenantEmailConfigInput.Normalize(dto, configuration);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (values.IsActive)
            {
                await UnitOfWork.TenantEmailConfigRepository
                    .DeactivateOtherActiveAsync(scope.TenantId, configuration.Id, cancellationToken);
            }

            values.ApplyTo(configuration);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await UnitOfWork.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Updated Tenant SMTP configuration {TenantEmailConfigId} for Tenant {TenantId}.", configuration.Id, scope.TenantId);
            return ApiResponse<TenantEmailConfigResponseDTO>.Success(
                MapResponse(mapper, configuration, scope),
                AppConstants.SuccessMessages.TenantEmailConfigUpdated);
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

public sealed class DeleteTenantEmailConfigCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    ILogger<DeleteTenantEmailConfigCommandHandler> logger)
    : TenantEmailConfigAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService),
        IRequestHandler<DeleteTenantEmailConfigCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteTenantEmailConfigCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var scope = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var configuration = await UnitOfWork.TenantEmailConfigRepository
            .GetForUpdateAsync(scope.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantEmailConfigNotFound);
        if (configuration.IsActive)
        {
            throw new ConflictException(AppConstants.ErrorMessages.TenantEmailConfigMustBeInactiveToDelete);
        }

        UnitOfWork.TenantEmailConfigRepository.Remove(configuration);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted inactive Tenant SMTP configuration {TenantEmailConfigId} for Tenant {TenantId}.", configuration.Id, scope.TenantId);
        return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.TenantEmailConfigDeleted);
    }
}

public sealed class GetTenantEmailConfigByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IMapper mapper)
    : TenantEmailConfigAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService),
        IRequestHandler<GetTenantEmailConfigByIdQuery, ApiResponse<TenantEmailConfigResponseDTO>>
{
    public async Task<ApiResponse<TenantEmailConfigResponseDTO>> Handle(
        GetTenantEmailConfigByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var scope = await ResolveTenantScopeAsync(request.AccessRequest, cancellationToken);
        var configuration = await UnitOfWork.TenantEmailConfigRepository
            .GetByIdAsync(scope.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantEmailConfigNotFound);
        return ApiResponse<TenantEmailConfigResponseDTO>.Success(
            MapResponse(mapper, configuration, scope),
            AppConstants.SuccessMessages.TenantEmailConfigRetrieved);
    }
}

public sealed class GetAllTenantEmailConfigsQueryHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IMapper mapper)
    : TenantEmailConfigAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService),
        IRequestHandler<GetAllTenantEmailConfigsQuery, ApiResponse<List<TenantEmailConfigResponseDTO>>>
{
    public async Task<ApiResponse<List<TenantEmailConfigResponseDTO>>> Handle(
        GetAllTenantEmailConfigsQuery request,
        CancellationToken cancellationToken)
    {
        var scope = await ResolveTenantScopeAsync(request.Filter, cancellationToken);
        var configurations = await UnitOfWork.TenantEmailConfigRepository
            .GetAllAsync(scope.TenantId, cancellationToken);
        return ApiResponse<List<TenantEmailConfigResponseDTO>>.Success(
            configurations.Select(configuration => MapResponse(mapper, configuration, scope)).ToList(),
            AppConstants.SuccessMessages.TenantEmailConfigRetrieved);
    }
}

#endregion

internal sealed record TenantEmailConfigInput(
    string SmtpHost,
    int SmtpPort,
    string SmtpUsername,
    string SmtpPassword,
    string FromEmail,
    string FromName,
    bool IsActive)
{
    public static TenantEmailConfigInput Normalize(CreateTenantEmailConfigRequestDTO dto) =>
        Create(dto.SmtpHost, dto.SmtpPort, dto.SmtpUsername, dto.SmtpPassword, dto.FromEmail, dto.FromName, dto.IsActive);

    public static TenantEmailConfigInput Normalize(
        UpdateTenantEmailConfigRequestDTO dto,
        TenantEmailConfig existing) =>
        Create(
            dto.SmtpHost,
            dto.SmtpPort,
            dto.SmtpUsername,
            string.IsNullOrWhiteSpace(dto.SmtpPassword)
                ? !string.IsNullOrWhiteSpace(existing.SecrateKey)
                    ? existing.SecrateKey
                    : existing.SmtpPasswordEncrypted
                : dto.SmtpPassword,
            dto.FromEmail,
            dto.FromName,
            dto.IsActive);

    public TenantEmailConfig ToEntity(long tenantId) => new()
    {
        TenantId = tenantId,
        SmtpHost = SmtpHost,
        SmtpPort = SmtpPort,
        SmtpUsername = SmtpUsername,
        SmtpPasswordEncrypted = SmtpPassword,
        SecrateKey = SmtpPassword,
        FromEmail = FromEmail,
        FromName = FromName,
        IsActive = IsActive
    };

    public void ApplyTo(TenantEmailConfig entity)
    {
        entity.SmtpHost = SmtpHost;
        entity.SmtpPort = SmtpPort;
        entity.SmtpUsername = SmtpUsername;
        entity.SmtpPasswordEncrypted = SmtpPassword;
        entity.SecrateKey = SmtpPassword;
        entity.FromEmail = FromEmail;
        entity.FromName = FromName;
        entity.IsActive = IsActive;
    }

    private static TenantEmailConfigInput Create(
        string? smtpHost,
        int smtpPort,
        string? smtpUsername,
        string? smtpPassword,
        string? fromEmail,
        string? fromName,
        bool isActive)
    {
        var normalizedHost = RequireAndTrim(smtpHost, 200);
        var normalizedUsername = RequireAndTrim(smtpUsername, 200);
        var normalizedPassword = RequireAndTrim(smtpPassword, 100);
        var normalizedFromEmail = RequireAndTrim(fromEmail, 200);
        var normalizedFromName = RequireAndTrim(fromName, 100);

        if (smtpPort is < 1 or > 65535 ||
            normalizedHost.Any(char.IsWhiteSpace) ||
            normalizedUsername.Any(char.IsWhiteSpace) ||
            Uri.CheckHostName(normalizedHost) is UriHostNameType.Unknown ||
            !IsValidEmail(normalizedFromEmail))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        return new TenantEmailConfigInput(
            normalizedHost,
            smtpPort,
            normalizedUsername,
            normalizedPassword,
            normalizedFromEmail,
            normalizedFromName,
            isActive);
    }

    private static string RequireAndTrim(string? value, int maximumLength)
    {
        var normalized = value?.Trim().Trim('\u200B', '\uFEFF');
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > maximumLength)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        return normalized;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var parsed = new MailAddress(email);
            return string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
