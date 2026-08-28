// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates a Host-managed Tenant and its selected configuration records atomically.
// ================================================================

using System.Text.RegularExpressions;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

/// <summary>
/// Represents the Host-managed request to update a Tenant and its selected configuration records using an encrypted route identifier.
/// </summary>
public sealed class UpdateNewTenantCommand : IRequest<ApiResponse<HostTenantResponseDTO>>
{
    /// <summary>Initializes a new instance of the <see cref="UpdateNewTenantCommand"/> class.</summary>
    public UpdateNewTenantCommand(
        string encryptedTenantId,
        NewTenantUpdateRequestDTO? requestDTO,
        PermissionRequestDTO? permissionRequest)
    {
        EncryptedTenantId = encryptedTenantId;
        RequestDTO = requestDTO;
        ModuleId = permissionRequest?.ModuleId ?? 0;
        OperationId = permissionRequest?.OperationId ?? 0;
    }

    /// <summary>Gets the encrypted Tenant identifier from the route.</summary>
    public string EncryptedTenantId { get; }

    /// <summary>Gets the submitted Tenant aggregate update request.</summary>
    public NewTenantUpdateRequestDTO? RequestDTO { get; }

    /// <summary>Gets the requested Host module identifier.</summary>
    public int ModuleId { get; }

    /// <summary>Gets the requested Host operation identifier.</summary>
    public int OperationId { get; }
}

/// <summary>
/// Handles the atomic Host-side update of a Tenant, its profile, one selected location, active email configuration, and active employee-code pattern.
/// </summary>
public sealed class UpdateNewTenantCommandHandler
    : IRequestHandler<UpdateNewTenantCommand, ApiResponse<HostTenantResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;

    /// <summary>Initializes a new instance of the <see cref="UpdateNewTenantCommandHandler"/> class.</summary>
    public UpdateNewTenantCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
    }

    /// <summary>Validates Host access and updates the requested Tenant aggregate in one database transaction.</summary>
    public async Task<ApiResponse<HostTenantResponseDTO>> Handle(
        UpdateNewTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (request?.RequestDTO is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var dto = request.RequestDTO;
        ValidateNestedRequest(dto);

        // This central validator directly permits Super Admin Hosts and validates module-operation
        // permission for every other Host user. No endpoint-specific authorization is added here.
        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            _commonRequestService,
            _unitOfWork.StoreProcedureRepository,
            request.ModuleId,
            request.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            request.EncryptedTenantId,
            hostContext.TenantEncryptionKey,
            _idEncoderService);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var tenant = await _unitOfWork.TenantRepository
                .GetHostManagedTenantByIdAsync(tenantId, cancellationToken)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            var profile = await _unitOfWork.TenantRepository
                .GetTenantProfileForUpdateAsync(tenantId, cancellationToken)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            var selectedLocation = await _unitOfWork.TenantLocationRepository
                .GetForUpdateAsync(tenantId, dto.SelectedLocation!.TenantLocationId, cancellationToken)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            var employeeCodePattern = await _unitOfWork.TenantEmployeeCodePatternRepository
                .GetActivePatternForUpdateAsync(tenantId, cancellationToken)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
            var emailConfiguration = await _unitOfWork.TenantEmailConfigRepository
                .GetActiveEmailConfigAsync(tenantId)
                ?? throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);

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

            var utcNow = DateTime.UtcNow;
            var tenantChanged = ApplyTenant(dto, tenant, submittedEmail, submittedCode);
            ApplyProfile(dto.Profile!, profile);
            var locationChanged = await ApplySelectedLocationAsync(
                dto.SelectedLocation!,
                selectedLocation,
                tenantId,
                cancellationToken);
            var employeeCodePatternChanged = ApplyEmployeeCodePattern(dto.EmployeeCodePattern!, employeeCodePattern);
            var emailConfigurationChanged = ApplyEmailConfiguration(dto.EmailConfiguration!, emailConfiguration, out var emailConfigurationUpdate);

            if (tenantChanged)
            {
                tenant.UpdatedById = hostContext.HostUserId;
                tenant.UpdatedDateTime = utcNow;
                await _unitOfWork.TenantRepository.StageHostManagedUpdateAsync(tenant, cancellationToken);
            }

            if (locationChanged)
            {
                selectedLocation.UpdatedById = hostContext.HostUserId;
                selectedLocation.UpdatedDateTime = utcNow;
            }

            if (employeeCodePatternChanged)
            {
                // LastUsedNumber deliberately remains untouched, preserving the issued employee-code sequence.
                employeeCodePattern.UpdatedById = hostContext.HostUserId;
                employeeCodePattern.UpdatedDateTime = utcNow;
                await _unitOfWork.TenantEmployeeCodePatternRepository.UpdatePatternAsync(employeeCodePattern);
            }

            if (emailConfigurationChanged)
            {
                // The repository preserves SecrateKey; it is never accepted or assigned by this update flow.
                await _unitOfWork.TenantEmailConfigRepository.UpdateEmailConfigAsync(emailConfigurationUpdate);
            }

            // Profile and selected-location lookups are tracked. EF persists only their detected changes.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return ApiResponse<HostTenantResponseDTO>.Success(
                MapTenant(tenant, hostContext.TenantEncryptionKey),
                AppConstants.SuccessMessages.TenantUpdatedSuccessfully);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static void ValidateNestedRequest(NewTenantUpdateRequestDTO dto)
    {
        if (dto.Profile is null ||
            dto.SelectedLocation is null ||
            dto.SelectedLocation.TenantLocationId <= 0 ||
            dto.EmployeeCodePattern is null ||
            dto.EmailConfiguration is null)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }
    }

    private static bool ApplyTenant(
        NewTenantUpdateRequestDTO dto,
        Tenant tenant,
        string? submittedEmail,
        string? submittedCode)
    {
        var changed = false;

        if (dto.TenantIndustryId > 0 && tenant.TenantIndustryId != dto.TenantIndustryId)
        {
            tenant.TenantIndustryId = dto.TenantIndustryId;
            changed = true;
        }

        changed |= AssignTrimmedIfProvided(dto.CompanyName, tenant.CompanyName, value => tenant.CompanyName = value);
        changed |= AssignTrimmedIfProvided(submittedCode, tenant.TenantCode, value => tenant.TenantCode = value);
        changed |= AssignTrimmedIfProvided(dto.CompanyEmailDomain, tenant.CompanyEmailDomain, value => tenant.CompanyEmailDomain = value);
        changed |= AssignTrimmedIfProvided(submittedEmail, tenant.TenantEmail, value => tenant.TenantEmail = value);

        if (dto.ContactPersonName is not null)
        {
            changed |= AssignValue(dto.ContactPersonName.Trim(), tenant.ContactPersonName, value => tenant.ContactPersonName = value);
        }

        if (dto.GenderId.HasValue && tenant.GenderId != dto.GenderId)
        {
            tenant.GenderId = dto.GenderId;
            changed = true;
        }

        if (dto.ContactNumber is not null)
        {
            changed |= AssignValue(dto.ContactNumber.Trim(), tenant.ContactNumber, value => tenant.ContactNumber = value);
        }

        if (dto.CountryId > 0 && tenant.CountryId != dto.CountryId)
        {
            tenant.CountryId = dto.CountryId;
            changed = true;
        }

        if (dto.DefaultCurrency.HasValue && tenant.DefaultCurrency != dto.DefaultCurrency)
        {
            tenant.DefaultCurrency = dto.DefaultCurrency;
            changed = true;
        }

        return changed;
    }

    private static bool ApplyProfile(NewTenantProfileUpdateRequestDTO dto, TenantProfile profile)
    {
        var changed = false;
        if (dto.Address is not null) changed |= AssignValue(dto.Address.Trim(), profile.Address, value => profile.Address = value);
        if (dto.LogoUrl is not null) changed |= AssignValue(dto.LogoUrl.Trim(), profile.LogoUrl, value => profile.LogoUrl = value);
        if (dto.ThemeColor is not null) changed |= AssignValue(dto.ThemeColor.Trim(), profile.ThemeColor, value => profile.ThemeColor = value);
        if (dto.BusinessType is not null) changed |= AssignValue(dto.BusinessType.Trim(), profile.BusinessType, value => profile.BusinessType = value);
        if (dto.Industry is not null) changed |= AssignValue(dto.Industry.Trim(), profile.Industry, value => profile.Industry = value);
        if (dto.TotalEmployees.HasValue && profile.TotalEmployees != dto.TotalEmployees) { profile.TotalEmployees = dto.TotalEmployees; changed = true; }
        if (dto.TotalBranches.HasValue && profile.TotalBranches != dto.TotalBranches) { profile.TotalBranches = dto.TotalBranches; changed = true; }
        if (dto.FoundedYear.HasValue && profile.FoundedYear != dto.FoundedYear) { profile.FoundedYear = dto.FoundedYear; changed = true; }
        if (dto.WebsiteUrl is not null) changed |= AssignValue(dto.WebsiteUrl.Trim(), profile.WebsiteUrl, value => profile.WebsiteUrl = value);
        return changed;
    }

    private async Task<bool> ApplySelectedLocationAsync(
        NewTenantSelectedLocationUpdateRequestDTO dto,
        TenantLocation location,
        long tenantId,
        CancellationToken cancellationToken)
    {
        if (dto.LocationType.HasValue && !Enum.IsDefined(dto.LocationType.Value))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var locationCode = dto.LocationCode is null ? null : RequireNonEmpty(dto.LocationCode, nameof(dto.LocationCode));
        var locationName = dto.LocationName is null ? null : RequireNonEmpty(dto.LocationName, nameof(dto.LocationName));
        var timeZoneId = dto.TimeZoneId is null ? null : RequireNonEmpty(dto.TimeZoneId, nameof(dto.TimeZoneId));
        if (dto.CountryId is <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        var finalCountryId = dto.CountryId ?? location.CountryId;
        var finalStateId = dto.StateId ?? location.StateId;
        var finalCityId = dto.CityId ?? location.CityId;
        if ((dto.CountryId.HasValue || dto.StateId.HasValue || dto.CityId.HasValue) &&
            !await _unitOfWork.TenantLocationRepository.IsValidGeographyAsync(
                finalCountryId,
                finalStateId,
                finalCityId,
                cancellationToken))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        }

        if (locationCode is not null &&
            !string.Equals(locationCode, location.LocationCode, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.TenantLocationRepository.LocationCodeExistsAsync(
                tenantId,
                locationCode,
                location.Id,
                cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.DuplicateTenantLocationCode);
        }

        var changed = false;
        if (locationCode is not null) changed |= AssignValue(locationCode, location.LocationCode, value => location.LocationCode = value);
        if (locationName is not null) changed |= AssignValue(locationName, location.LocationName, value => location.LocationName = value);
        if (dto.LocationType.HasValue && location.LocationType != (short)dto.LocationType.Value) { location.LocationType = (short)dto.LocationType.Value; changed = true; }
        if (dto.CountryId.HasValue && location.CountryId != dto.CountryId.Value) { location.CountryId = dto.CountryId.Value; changed = true; }
        if (dto.StateId.HasValue && location.StateId != dto.StateId) { location.StateId = dto.StateId; changed = true; }
        if (dto.CityId.HasValue && location.CityId != dto.CityId) { location.CityId = dto.CityId; changed = true; }
        if (dto.Address is not null) changed |= AssignValue(dto.Address.Trim(), location.Address, value => location.Address = value);
        if (dto.Landmark is not null) changed |= AssignValue(dto.Landmark.Trim(), location.Landmark, value => location.Landmark = value);
        if (dto.PostalCode is not null) changed |= AssignValue(dto.PostalCode.Trim(), location.PostalCode, value => location.PostalCode = value);
        if (dto.Latitude.HasValue && location.Latitude != dto.Latitude) { location.Latitude = dto.Latitude; changed = true; }
        if (dto.Longitude.HasValue && location.Longitude != dto.Longitude) { location.Longitude = dto.Longitude; changed = true; }
        if (dto.GeoFenceRadiusMeters.HasValue && location.GeoFenceRadiusMeters != dto.GeoFenceRadiusMeters) { location.GeoFenceRadiusMeters = dto.GeoFenceRadiusMeters; changed = true; }
        if (timeZoneId is not null) changed |= AssignValue(timeZoneId, location.TimeZoneId, value => location.TimeZoneId = value);
        if (dto.IsGeoFenceEnabled.HasValue && location.IsGeoFenceEnabled != dto.IsGeoFenceEnabled.Value) { location.IsGeoFenceEnabled = dto.IsGeoFenceEnabled.Value; changed = true; }
        if (dto.IsAttendanceAllowed.HasValue && location.IsAttendanceAllowed != dto.IsAttendanceAllowed.Value) { location.IsAttendanceAllowed = dto.IsAttendanceAllowed.Value; changed = true; }
        if (dto.IsBiometricEnabled.HasValue && location.IsBiometricEnabled != dto.IsBiometricEnabled.Value) { location.IsBiometricEnabled = dto.IsBiometricEnabled.Value; changed = true; }
        return changed;
    }

    private static bool ApplyEmployeeCodePattern(
        NewTenantEmployeeCodePatternUpdateRequestDTO dto,
        EmployeeCodePattern pattern)
    {
        var changed = false;
        if (dto.Prefix is not null)
        {
            var prefix = RequireNonEmpty(dto.Prefix, nameof(dto.Prefix)).ToUpperInvariant();
            if (prefix.Length > 10 || !Regex.IsMatch(prefix, "^[A-Z]+$"))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            changed |= AssignValue(prefix, pattern.Prefix, value => pattern.Prefix = value);
        }

        if (dto.Separator is not null)
        {
            var separator = RequireNonEmpty(dto.Separator, nameof(dto.Separator));
            if (separator is not ("_" or "/" or "-"))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            changed |= AssignValue(separator, pattern.Separator, value => pattern.Separator = value);
        }

        if (dto.RunningNumberLength is not null)
        {
            if (!int.TryParse(dto.RunningNumberLength, out var runningNumberLength) ||
                runningNumberLength is < 3 or > 7)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            if (pattern.RunningNumberLength != runningNumberLength)
            {
                pattern.RunningNumberLength = runningNumberLength;
                changed = true;
            }
        }

        if (dto.IncludeYear.HasValue && pattern.IncludeYear != dto.IncludeYear.Value) { pattern.IncludeYear = dto.IncludeYear.Value; changed = true; }
        if (dto.IncludeMonth.HasValue && pattern.IncludeMonth != dto.IncludeMonth.Value) { pattern.IncludeMonth = dto.IncludeMonth.Value; changed = true; }
        if (dto.IncludeDepartment.HasValue && pattern.IncludeDepartment != dto.IncludeDepartment.Value) { pattern.IncludeDepartment = dto.IncludeDepartment.Value; changed = true; }
        return changed;
    }

    private static bool ApplyEmailConfiguration(
        NewTenantEmailConfigurationUpdateRequestDTO dto,
        TenantEmailConfig existing,
        out TenantEmailConfig update)
    {
        var smtpPassword = string.IsNullOrWhiteSpace(dto.SmtpPasswordEncrypted)
            ? existing.SmtpPasswordEncrypted
            : dto.SmtpPasswordEncrypted;
        update = new TenantEmailConfig
        {
            Id = existing.Id,
            SmtpHost = dto.SmtpHost is null ? existing.SmtpHost : dto.SmtpHost.Trim(),
            SmtpPort = dto.SmtpPort ?? existing.SmtpPort,
            SmtpUsername = dto.SmtpUsername is null ? existing.SmtpUsername : dto.SmtpUsername.Trim(),
            SmtpPasswordEncrypted = smtpPassword,
            FromEmail = dto.FromEmail is null ? existing.FromEmail : dto.FromEmail.Trim(),
            FromName = dto.FromName is null ? existing.FromName : dto.FromName.Trim(),
            IsActive = dto.IsActive ?? existing.IsActive
        };

        return !string.Equals(update.SmtpHost, existing.SmtpHost, StringComparison.Ordinal) ||
               update.SmtpPort != existing.SmtpPort ||
               !string.Equals(update.SmtpUsername, existing.SmtpUsername, StringComparison.Ordinal) ||
               !string.Equals(update.SmtpPasswordEncrypted, existing.SmtpPasswordEncrypted, StringComparison.Ordinal) ||
               !string.Equals(update.FromEmail, existing.FromEmail, StringComparison.Ordinal) ||
               !string.Equals(update.FromName, existing.FromName, StringComparison.Ordinal) ||
               update.IsActive != existing.IsActive;
    }

    private static bool AssignTrimmedIfProvided(string? submittedValue, string? existingValue, Action<string> assign)
    {
        if (string.IsNullOrWhiteSpace(submittedValue))
        {
            return false;
        }

        return AssignValue(submittedValue.Trim(), existingValue, assign);
    }

    private static bool AssignValue(string value, string? existingValue, Action<string> assign)
    {
        if (string.Equals(value, existingValue, StringComparison.Ordinal))
        {
            return false;
        }

        assign(value);
        return true;
    }

    private static string RequireNonEmpty(string value, string propertyName)
    {
        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }

        return trimmed;
    }

    private HostTenantResponseDTO MapTenant(Tenant tenant, string tenantEncryptionKey) =>
        new()
        {
            Id = HostTenantIdentifierProtector.Encrypt(tenant.Id, tenantEncryptionKey, _idEncoderService),
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
