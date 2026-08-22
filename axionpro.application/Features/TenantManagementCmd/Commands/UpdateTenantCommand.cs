// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Host-side command contract for updating editable Tenant fields.
// ================================================================

using axionpro.application.DTOs.Tenant;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

#region Command

/// <summary>
/// Represents the Host-side request to update editable Tenant fields.
/// </summary>
/// <remarks>
/// The future handler must validate the Host with <c>ValidateHostUserRequestAsync()</c> and derive
/// all actor and audit information server-side. No Host user identifier or audit value is accepted from the client.
/// </remarks>
public sealed class UpdateTenantCommand : IRequest<ApiResponse<TenantResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTenantCommand"/> class.
    /// </summary>
    /// <param name="requestDTO">The Tenant update request.</param>
    public UpdateTenantCommand(UpdateTenantRequestDTO requestDTO)
    {
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the Tenant update request.
    /// </summary>
    public UpdateTenantRequestDTO RequestDTO { get; }
}

#endregion

#region Host-Managed Route Command

/// <summary>
/// Represents the Host-managed route request to update Tenant details using an authoritative route identifier.
/// </summary>
public sealed class UpdateHostManagedTenantCommand : IRequest<ApiResponse<TenantResponseDTO>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateHostManagedTenantCommand"/> class.
    /// </summary>
    /// <param name="tenantId">The authoritative Tenant identifier from the route.</param>
    /// <param name="requestDTO">The client-editable Tenant details.</param>
    public UpdateHostManagedTenantCommand(long tenantId, UpdateHostManagedTenantRequestDTO? requestDTO)
    {
        TenantId = tenantId;
        RequestDTO = requestDTO;
    }

    /// <summary>
    /// Gets the authoritative Tenant identifier from the route.
    /// </summary>
    public long TenantId { get; }

    /// <summary>
    /// Gets the client-editable Tenant details.
    /// </summary>
    public UpdateHostManagedTenantRequestDTO? RequestDTO { get; }
}

#endregion

#region Host-Managed Route Handler

/// <summary>
/// Validates and updates Host-managed Tenant information using trusted audit context.
/// </summary>
public sealed class UpdateHostManagedTenantCommandHandler
    : IRequestHandler<UpdateHostManagedTenantCommand, ApiResponse<TenantResponseDTO>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateHostManagedTenantCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides Tenant persistence and transaction operations.</param>
    /// <param name="commonRequestService">Validates the current Host principal.</param>
    public UpdateHostManagedTenantCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Updates editable Tenant fields while preserving lifecycle and verification state.
    /// </summary>
    /// <param name="request">The Host-managed Tenant update command.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>The updated Tenant response.</returns>
    /// <exception cref="ValidationErrorException">Thrown when the route identifier or request is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the Tenant is unavailable or soft deleted.</exception>
    /// <exception cref="ConflictException">Thrown when another active Tenant owns the submitted email or code.</exception>
    public async Task<ApiResponse<TenantResponseDTO>> Handle(
        UpdateHostManagedTenantCommand request,
        CancellationToken cancellationToken)
    {
        // Validate the current Host identity before accepting editable Tenant data.
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync();

        if (request?.RequestDTO is null || request.TenantId <= 0 || hostUserId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantByIdAsync(request.TenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        var dto = request.RequestDTO;
        var submittedEmail = dto.TenantEmail?.Trim();
        var submittedCode = dto.TenantCode?.Trim();

        if (!string.IsNullOrWhiteSpace(submittedEmail) &&
            !string.Equals(submittedEmail, tenant.TenantEmail, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.TenantRepository.IsTenantEmailInUseAsync(submittedEmail, tenant.Id, cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.ResourceConflict);
        }

        if (!string.IsNullOrWhiteSpace(submittedCode) &&
            !string.Equals(submittedCode, tenant.TenantCode, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.TenantRepository.IsTenantCodeInUseAsync(submittedCode, tenant.Id, cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.ResourceConflict);
        }

        // Preserve omitted optional fields and all server-controlled verification and lifecycle values.
        if (dto.TenantIndustryId > 0)
        {
            tenant.TenantIndustryId = dto.TenantIndustryId;
        }

        if (!string.IsNullOrWhiteSpace(dto.CompanyName))
        {
            tenant.CompanyName = dto.CompanyName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(submittedCode))
        {
            tenant.TenantCode = submittedCode;
        }

        if (!string.IsNullOrWhiteSpace(dto.CompanyEmailDomain))
        {
            tenant.CompanyEmailDomain = dto.CompanyEmailDomain.Trim();
        }

        if (!string.IsNullOrWhiteSpace(submittedEmail))
        {
            tenant.TenantEmail = submittedEmail;
        }

        if (dto.ContactPersonName is not null)
        {
            tenant.ContactPersonName = dto.ContactPersonName.Trim();
        }

        if (dto.GenderId.HasValue)
        {
            tenant.GenderId = dto.GenderId;
        }

        if (dto.ContactNumber is not null)
        {
            tenant.ContactNumber = dto.ContactNumber.Trim();
        }

        if (dto.CountryId > 0)
        {
            tenant.CountryId = dto.CountryId;
        }

        if (dto.DefaultCurrency.HasValue)
        {
            tenant.DefaultCurrency = dto.DefaultCurrency;
        }

        tenant.UpdatedById = hostUserId;
        tenant.UpdatedDateTime = DateTime.UtcNow;

        await _unitOfWork.TenantRepository.StageHostManagedUpdateAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<TenantResponseDTO>.Success(
            MapTenant(tenant),
            AppConstants.SuccessMessages.TenantUpdatedSuccessfully);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Maps a persisted Tenant to the established client response model.
    /// </summary>
    /// <param name="tenant">The Tenant to map.</param>
    /// <returns>The mapped Tenant response.</returns>
    private static TenantResponseDTO MapTenant(Tenant tenant)
    {
        return new TenantResponseDTO
        {
            Id = tenant.Id,
            CompanyName = tenant.CompanyName,
            TenantCode = tenant.TenantCode,
            CompanyEmailDomain = tenant.CompanyEmailDomain,
            TenantEmail = tenant.TenantEmail,
            ContactPersonName = tenant.ContactPersonName,
            ContactNumber = tenant.ContactNumber,
            CountryId = tenant.CountryId,
            IsVerified = tenant.IsVerified,
            IsActive = tenant.IsActive
        };
    }

    #endregion
}

#endregion
