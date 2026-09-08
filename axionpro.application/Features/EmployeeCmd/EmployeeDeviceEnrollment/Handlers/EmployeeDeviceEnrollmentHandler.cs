// ================================================================
// Purpose : Tenant-safe employee/device enrollment. Device enrollid is the
//           decrypted database Employee.Id, never a UI supplied value.
// ================================================================

using System.Globalization;
using System.Text.Json;
using AutoMapper;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Exceptions;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IDeviceCommunication;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using EmployeeDeviceEnrollmentEntity = axionpro.domain.Entity.EmployeeDeviceEnrollment;

namespace axionpro.application.Features.EmployeeCmd.EmployeeDeviceEnrollment.Handlers;

public sealed class CreateEmployeeDeviceEnrollmentCommand(CreateEmployeeDeviceEnrollmentRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public CreateEmployeeDeviceEnrollmentRequestDTO DTO { get; } = dto; }
public sealed class UpdateEmployeeDeviceEnrollmentCommand(UpdateEmployeeDeviceEnrollmentRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public UpdateEmployeeDeviceEnrollmentRequestDTO DTO { get; } = dto; }
public sealed class DeleteEmployeeDeviceEnrollmentCommand(string id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<bool>> { public string Id { get; } = id; public PermissionRequestDTO PermissionRequest { get; } = permissionRequest; }
public sealed class UpdateEmployeeDeviceEnrollmentStatusCommand(UpdateEmployeeDeviceEnrollmentStatusRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public UpdateEmployeeDeviceEnrollmentStatusRequestDTO DTO { get; } = dto; }
public sealed class GetEmployeeDeviceEnrollmentByIdQuery(string id, PermissionRequestDTO permissionRequest) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public string Id { get; } = id; public PermissionRequestDTO PermissionRequest { get; } = permissionRequest; }
public sealed class GetEmployeeDeviceEnrollmentsQuery(EmployeeDeviceEnrollmentFilterRequestDTO filter) : IRequest<ApiResponse<List<EmployeeDeviceEnrollmentResponseDTO>>> { public EmployeeDeviceEnrollmentFilterRequestDTO Filter { get; } = filter; }

public abstract class EmployeeDeviceEnrollmentHandlerBase(IUnitOfWork unitOfWork, ICommonRequestService common, ILogger<TenantConfigurationHandlerBase> logger, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : TenantConfigurationHandlerBase(unitOfWork, common, logger, ids)
{
    protected IIdEncoderService Ids { get; } = ids;
    protected IDeviceCommandSubmissionService Commands { get; } = commands;
    protected IEncryptionService Encryption { get; } = encryption;

    protected long Decode(string? value, string key, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ValidationErrorException($"A valid {name} is required.");
        try
        {
            var id = Ids.DecodeId_long(value.Trim(), key);
            if (id <= 0 || Ids.EncodeId_long(id, key) != value.Trim()) throw new ValidationErrorException($"A valid {name} is required.");
            return id;
        }
        catch (ValidationErrorException) { throw; }
        catch { throw new ValidationErrorException($"A valid {name} is required."); }
    }

    protected async Task<(CommonDecodedResult Context, long Id)> EnrollmentContext(string id, CancellationToken ct)
    {
        var context = await ValidateTenantDataAccessContextAsync();
        return (context, Decode(id, context.Claims.TenantEncriptionKey, "employee device enrollment identifier"));
    }

    protected static void ValidateWindows(IEnumerable<EmployeeDeviceAccessWindowRequestDTO> windows, DateTime? from, DateTime? to)
    {
        if (from.HasValue && to.HasValue && from > to) throw new ValidationErrorException("Access validity end must be later than its start.");
        foreach (var item in windows)
            if (!Enum.IsDefined(item.DayOfWeek) || item.StartLocalTime >= item.EndLocalTime)
                throw new ValidationErrorException("Each working access window needs a valid day and start/end time.");
    }

    protected static void ReplaceWindows(EmployeeDeviceEnrollmentEntity e, IEnumerable<EmployeeDeviceAccessWindowRequestDTO> windows, long actorId)
    {
        var now = DateTime.UtcNow;
        foreach (var old in e.EmployeeDeviceAccessWindows.Where(x => !x.IsSoftDeleted)) { old.IsSoftDeleted = true; old.IsActive = false; old.SoftDeletedById = actorId; old.SoftDeletedDateTime = now; }
        foreach (var item in windows) e.EmployeeDeviceAccessWindows.Add(new EmployeeDeviceAccessWindow { DayOfWeek = (short)item.DayOfWeek, StartLocalTime = item.StartLocalTime, EndLocalTime = item.EndLocalTime, IsActive = item.IsActive, AddedById = actorId, AddedDateTime = now });
    }

    protected async Task QueueBaselineAsync(EmployeeDeviceEnrollmentEntity e, long actorId, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(new
        {
            cmd = DeviceCommands.SetUserInfo,
            enrollid = long.Parse(e.EnrollId, CultureInfo.InvariantCulture),
            aliasid = e.Employee.EmployementCode ?? e.EnrollId,
            name = string.Join(" ", new[] { e.Employee.FirstName, e.Employee.MiddleName, e.Employee.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
            department = e.Employee.DepartmentId?.ToString(CultureInfo.InvariantCulture) ?? "0",
            admin = 0, // privilege is always User; Angular cannot alter it.
            enable = e.IsActive ? 1 : 0
        });
        await Commands.SubmitAsync(new DeviceCommandSubmission(e.TenantId, e.TenantDeviceId, DeviceCommands.SetUserInfo, payload, actorId, ProtectPayload: true), ct);
    }

    protected EmployeeDeviceEnrollmentResponseDTO ToResponse(IMapper mapper, EmployeeDeviceEnrollmentEntity e, string key)
    {
        var dto = mapper.Map<EmployeeDeviceEnrollmentResponseDTO>(e);
        dto.Id = Ids.EncodeId_long(e.Id, key); dto.EmployeeId = Ids.EncodeId_long(e.EmployeeId, key); dto.TenantDeviceId = Ids.EncodeId_long(e.TenantDeviceId, key); dto.TenantLocationId = Ids.EncodeId_long(e.TenantLocationId, key);
        if (e.TenantCardMasterId.HasValue) dto.TenantCardId = Ids.EncodeId_long(e.TenantCardMasterId.Value, key);
        if (e.TenantCardMaster is not null)
        {
            var value = Encryption.Decrypt(e.TenantCardMaster.CardNumberEncrypted, key);
            dto.MaskedCardNumber = value.Length <= 4 ? "****" : $"****{value[^4..]}";
        }
        dto.FaceCommandStatus = e.FaceDeviceCommand is null ? null : (DeviceCommandStatus)e.FaceDeviceCommand.Status;
        dto.CardCommandStatus = e.CardDeviceCommand is null ? null : (DeviceCommandStatus)e.CardDeviceCommand.Status;
        dto.PinCommandStatus = e.PinDeviceCommand is null ? null : (DeviceCommandStatus)e.PinDeviceCommand.Status;
        dto.UserActivationCommandStatus = e.UserActivationDeviceCommand is null ? null : (DeviceCommandStatus)e.UserActivationDeviceCommand.Status;
        return dto;
    }
}

public sealed class CreateEmployeeDeviceEnrollmentCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<CreateEmployeeDeviceEnrollmentCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(CreateEmployeeDeviceEnrollmentCommand request, CancellationToken ct)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing);
        var context = await ValidateTenantDataAccessContextAsync();
        var employeeId = Decode(dto.EmployeeId, context.Claims.TenantEncriptionKey, "EmployeeId"); var deviceId = Decode(dto.TenantDeviceId, context.Claims.TenantEncriptionKey, "TenantDeviceId");
        await EnsureEmployeeDataAccessAsync(context, employeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); ValidateWindows(dto.AccessWindows, dto.AccessEffectiveFromDateTime, dto.AccessEffectiveToDateTime);
        var device = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetEligibleTenantDeviceAsync(context.TenantId, deviceId, ct) ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidTenantConfigurationReference);
        if (!await UnitOfWork.EmployeeDeviceEnrollmentRepository.IsEligibleEmployeeAsync(context.TenantId, employeeId, ct) || await UnitOfWork.EmployeeDeviceEnrollmentRepository.EnrollmentExistsAsync(context.TenantId, employeeId, deviceId, null, ct) || !await UnitOfWork.EmployeeDeviceEnrollmentRepository.HasEligibleEmployeeLocationAssignmentAsync(context.TenantId, employeeId, device.TenantLocationId, DateOnly.FromDateTime(dto.AccessEffectiveFromDateTime ?? DateTime.UtcNow), ct)) throw new ValidationErrorException("The employee, device, or employee-location assignment is not eligible.");
        var entity = new EmployeeDeviceEnrollmentEntity { TenantId = context.TenantId, EmployeeId = employeeId, TenantDeviceId = deviceId, TenantLocationId = device.TenantLocationId, EnrollId = employeeId.ToString(CultureInfo.InvariantCulture), IsActive = dto.IsActive, AccessEffectiveFromDateTime = dto.AccessEffectiveFromDateTime, AccessEffectiveToDateTime = dto.AccessEffectiveToDateTime, AddedById = context.LoggedInEmployeeId, AddedDateTime = DateTime.UtcNow };
        ReplaceWindows(entity, dto.AccessWindows, context.LoggedInEmployeeId); await UnitOfWork.EmployeeDeviceEnrollmentRepository.AddAsync(entity, ct); await UnitOfWork.SaveChangesAsync(ct);
        var tracked = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, entity.Id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound);
        await QueueBaselineAsync(tracked, context.LoggedInEmployeeId, ct);
        var read = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, entity.Id, ct) ?? tracked;
        Logger.LogInformation("Queued user enrollment for Employee {EmployeeId} on TenantDevice {TenantDeviceId}.", employeeId, deviceId);
        return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, read, context.Claims.TenantEncriptionKey), AppConstants.SuccessMessages.EmployeeDeviceEnrollmentCreated);
    }
}

public sealed class UpdateEmployeeDeviceEnrollmentCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<UpdateEmployeeDeviceEnrollmentCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(UpdateEmployeeDeviceEnrollmentCommand request, CancellationToken ct)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing); var (context, id) = await EnrollmentContext(dto.Id, ct); var entity = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound);
        await EnsureEmployeeDataAccessAsync(context, entity.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct);
        if (Decode(dto.EmployeeId, context.Claims.TenantEncriptionKey, "EmployeeId") != entity.EmployeeId || Decode(dto.TenantDeviceId, context.Claims.TenantEncriptionKey, "TenantDeviceId") != entity.TenantDeviceId) throw new ValidationErrorException("Create a new enrollment to change its employee or physical device.");
        if (dto.IsActive != entity.IsActive) throw new ValidationErrorException("Use update-status to enable or disable a device user.");
        ValidateWindows(dto.AccessWindows, dto.AccessEffectiveFromDateTime, dto.AccessEffectiveToDateTime); entity.AccessEffectiveFromDateTime = dto.AccessEffectiveFromDateTime; entity.AccessEffectiveToDateTime = dto.AccessEffectiveToDateTime; entity.UpdatedById = context.LoggedInEmployeeId; entity.UpdatedDateTime = DateTime.UtcNow; ReplaceWindows(entity, dto.AccessWindows, context.LoggedInEmployeeId); await UnitOfWork.SaveChangesAsync(ct);
        var read = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, id, ct) ?? entity; return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, read, context.Claims.TenantEncriptionKey), AppConstants.SuccessMessages.EmployeeDeviceEnrollmentUpdated);
    }
}

public sealed class UpdateEmployeeDeviceEnrollmentStatusCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<UpdateEmployeeDeviceEnrollmentStatusCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(UpdateEmployeeDeviceEnrollmentStatusCommand request, CancellationToken ct)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.RequiredDataMissing); var (context, id) = await EnrollmentContext(dto.Id, ct); var entity = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound); await EnsureEmployeeDataAccessAsync(context, entity.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct);
        entity.IsActive = dto.IsActive; entity.UpdatedById = context.LoggedInEmployeeId; entity.UpdatedDateTime = DateTime.UtcNow;
        var command = await Commands.SubmitAsync(new DeviceCommandSubmission(entity.TenantId, entity.TenantDeviceId, DeviceCommands.EnableUser, JsonSerializer.Serialize(new { cmd = DeviceCommands.EnableUser, enrollid = long.Parse(entity.EnrollId, CultureInfo.InvariantCulture), enflag = dto.IsActive }), context.LoggedInEmployeeId, ProtectPayload: true), ct);
        entity.UserActivationDeviceCommandId = command.DeviceCommandId;
        await UnitOfWork.SaveChangesAsync(ct);
        var read = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, id, ct) ?? entity; return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, read, context.Claims.TenantEncriptionKey), AppConstants.SuccessMessages.EmployeeDeviceEnrollmentStatusUpdated);
    }
}

public sealed class DeleteEmployeeDeviceEnrollmentCommandHandler(IUnitOfWork u, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<DeleteEmployeeDeviceEnrollmentCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteEmployeeDeviceEnrollmentCommand request, CancellationToken ct)
    {
        var (context, id) = await EnrollmentContext(request.Id, ct); var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound); await EnsureEmployeeDataAccessAsync(context, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct);
        await Commands.SubmitAsync(new DeviceCommandSubmission(e.TenantId, e.TenantDeviceId, DeviceCommands.DeleteUser, JsonSerializer.Serialize(new { cmd = DeviceCommands.DeleteUser, enrollid = long.Parse(e.EnrollId, CultureInfo.InvariantCulture), backupnum = 12 }), context.LoggedInEmployeeId, ProtectPayload: true), ct);
        if (e.TenantCardMaster is not null) e.TenantCardMaster.CardStatus = (short)TenantCardStatus.Available; e.IsActive = false; e.IsSoftDeleted = true; e.SoftDeletedById = context.LoggedInEmployeeId; e.SoftDeletedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmployeeDeviceEnrollmentDeleted);
    }
}

public sealed class GetEmployeeDeviceEnrollmentByIdQueryHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<GetEmployeeDeviceEnrollmentByIdQuery, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(GetEmployeeDeviceEnrollmentByIdQuery request, CancellationToken ct) { var (context, id) = await EnrollmentContext(request.Id, ct); var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, id, ct) ?? throw new NotFoundException(AppConstants.ErrorMessages.EmployeeDeviceEnrollmentNotFound); await EnsureEmployeeDataAccessAsync(context, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct); return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, e, context.Claims.TenantEncriptionKey)); }
}

public sealed class GetEmployeeDeviceEnrollmentsQueryHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<GetEmployeeDeviceEnrollmentsQuery, ApiResponse<List<EmployeeDeviceEnrollmentResponseDTO>>>
{
    public async Task<ApiResponse<List<EmployeeDeviceEnrollmentResponseDTO>>> Handle(GetEmployeeDeviceEnrollmentsQuery request, CancellationToken ct)
    {
        var filter = request.Filter ?? new(); var context = await ValidateTenantDataAccessContextAsync();
        if (!string.IsNullOrWhiteSpace(filter.EmployeeId)) { filter.ResolvedEmployeeId = Decode(filter.EmployeeId, context.Claims.TenantEncriptionKey, "EmployeeId"); await EnsureEmployeeDataAccessAsync(context, filter.ResolvedEmployeeId.Value, EmployeeDataAccessRequirement.PersonalDetails, ct); }
        if (!string.IsNullOrWhiteSpace(filter.TenantDeviceId)) filter.ResolvedTenantDeviceId = Decode(filter.TenantDeviceId, context.Claims.TenantEncriptionKey, "TenantDeviceId");
        var page = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetPagedAsync(context.TenantId, filter, context.LoggedInEmployeeId, context.RoleTypeId, ct); return Paged(page.Data.Select(e => ToResponse(m, e, context.Claims.TenantEncriptionKey)).ToList(), page.PageNumber, page.PageSize, page.TotalCount, "Employee device enrollments retrieved successfully.");
    }
}
