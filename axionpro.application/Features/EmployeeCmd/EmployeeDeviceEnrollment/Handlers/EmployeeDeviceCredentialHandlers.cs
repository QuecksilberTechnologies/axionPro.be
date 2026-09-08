// ================================================================
// Purpose : Queues write-only face, PIN and card credentials. Raw biometric
//           data and PINs are protected queue payloads and are never stored.
// ================================================================

using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using AutoMapper;
using axionpro.application.Constants;
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

namespace axionpro.application.Features.EmployeeCmd.EmployeeDeviceEnrollment.Handlers;

public sealed class UpsertEmployeeDeviceFaceCommand(UpsertEmployeeDeviceFaceRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public UpsertEmployeeDeviceFaceRequestDTO DTO { get; } = dto; }
public sealed class UpsertEmployeeDevicePinCommand(UpsertEmployeeDevicePinRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public UpsertEmployeeDevicePinRequestDTO DTO { get; } = dto; }
public sealed class BindEmployeeDeviceCardCommand(BindEmployeeDeviceCardRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public BindEmployeeDeviceCardRequestDTO DTO { get; } = dto; }
public sealed class RemoveEmployeeDeviceCredentialCommand(RemoveEmployeeDeviceCredentialRequestDTO dto) : IRequest<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> { public RemoveEmployeeDeviceCredentialRequestDTO DTO { get; } = dto; }

public sealed class UpsertEmployeeDeviceFaceCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<UpsertEmployeeDeviceFaceCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(UpsertEmployeeDeviceFaceCommand request, CancellationToken ct)
    {
        var dto = request.DTO ?? throw new ValidationErrorException("Face image is required."); var (context, id) = await EnrollmentContext(dto.EnrollmentId, ct);
        if (dto.FaceImage is null || dto.FaceImage.Length is <= 0 or > 2_000_000 || dto.FaceImage.ContentType is not ("image/jpeg" or "image/png")) throw new ValidationErrorException("Upload a JPEG or PNG face image no larger than 2 MB.");
        var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, id, ct) ?? throw new NotFoundException("Employee device enrollment was not found."); await EnsureEmployeeDataAccessAsync(context, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct);
        await using var stream = new MemoryStream(); await dto.FaceImage.CopyToAsync(stream, ct); var bytes = stream.ToArray();
        var command = await Commands.SubmitAsync(new DeviceCommandSubmission(e.TenantId, e.TenantDeviceId, DeviceCommands.SetUserInfo, JsonSerializer.Serialize(new { cmd = DeviceCommands.SetUserInfo, enrollid = long.Parse(e.EnrollId, CultureInfo.InvariantCulture), admin = 0, backupnum = 50, record = Convert.ToBase64String(bytes) }), context.LoggedInEmployeeId, ProtectPayload: true), ct);
        e.FaceImageHash = Convert.ToHexString(SHA256.HashData(bytes)); e.FaceDeviceCommandId = command.DeviceCommandId; e.FaceDeploymentStatus = (short)DeviceCredentialDeploymentStatus.Queued; e.UpdatedById = context.LoggedInEmployeeId; e.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct);
        var read = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, id, ct) ?? e; return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, read, context.Claims.TenantEncriptionKey), "Face enrollment has been queued for the device.");
    }
}

public sealed class UpsertEmployeeDevicePinCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<UpsertEmployeeDevicePinCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(UpsertEmployeeDevicePinCommand request, CancellationToken ct)
    {
        var dto = request.DTO ?? throw new ValidationErrorException("PIN is required."); if (dto.Pin.Length is < 4 or > 12 || dto.Pin.Any(x => !char.IsAsciiDigit(x))) throw new ValidationErrorException("Device PIN must contain 4 to 12 digits.");
        var (context, id) = await EnrollmentContext(dto.EnrollmentId, ct); var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, id, ct) ?? throw new NotFoundException("Employee device enrollment was not found."); await EnsureEmployeeDataAccessAsync(context, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct);
        var command = await Commands.SubmitAsync(new DeviceCommandSubmission(e.TenantId, e.TenantDeviceId, DeviceCommands.SetUserInfo, JsonSerializer.Serialize(new { cmd = DeviceCommands.SetUserInfo, enrollid = long.Parse(e.EnrollId, CultureInfo.InvariantCulture), admin = 0, backupnum = 10, pwd = dto.Pin }), context.LoggedInEmployeeId, ProtectPayload: true), ct);
        e.PinDeviceCommandId = command.DeviceCommandId; e.PinDeploymentStatus = (short)DeviceCredentialDeploymentStatus.Queued; e.UpdatedById = context.LoggedInEmployeeId; e.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); var read = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, id, ct) ?? e; return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, read, context.Claims.TenantEncriptionKey), "PIN enrollment has been queued for the device.");
    }
}

public sealed class BindEmployeeDeviceCardCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<BindEmployeeDeviceCardCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(BindEmployeeDeviceCardCommand request, CancellationToken ct)
    {
        var dto = request.DTO ?? throw new ValidationErrorException("Card is required."); var (context, id) = await EnrollmentContext(dto.EnrollmentId, ct); var cardId = Decode(dto.TenantCardId, context.Claims.TenantEncriptionKey, "TenantCardId");
        var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, id, ct) ?? throw new NotFoundException("Employee device enrollment was not found."); await EnsureEmployeeDataAccessAsync(context, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct);
        var card = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetCardForUpdateAsync(context.TenantId, cardId, ct) ?? throw new ValidationErrorException("The selected card is not available."); if (!card.IsActive || card.CardStatus != (short)TenantCardStatus.Available) throw new ValidationErrorException("The selected card is not available.");
        var value = Encryption.Decrypt(card.CardNumberEncrypted, context.Claims.TenantEncriptionKey); if (!long.TryParse(value, out _)) throw new ValidationErrorException("The selected card number cannot be sent to this device.");
        if (e.TenantCardMaster is not null) e.TenantCardMaster.CardStatus = (short)TenantCardStatus.Available; e.TenantCardMasterId = card.Id; e.TenantCardMaster = card; card.CardStatus = (short)TenantCardStatus.Assigned; e.CardDeploymentStatus = (short)DeviceCredentialDeploymentStatus.Queued; e.UpdatedById = context.LoggedInEmployeeId; e.UpdatedDateTime = DateTime.UtcNow;
        var command = await Commands.SubmitAsync(new DeviceCommandSubmission(e.TenantId, e.TenantDeviceId, DeviceCommands.SetUserInfo, JsonSerializer.Serialize(new { cmd = DeviceCommands.SetUserInfo, enrollid = long.Parse(e.EnrollId, CultureInfo.InvariantCulture), admin = 0, backupnum = 11, card = value }), context.LoggedInEmployeeId, ProtectPayload: true), ct); e.CardDeviceCommandId = command.DeviceCommandId; await UnitOfWork.SaveChangesAsync(ct);
        var read = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, id, ct) ?? e; return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, read, context.Claims.TenantEncriptionKey), "Card binding has been queued for the device.");
    }
}

public sealed class RemoveEmployeeDeviceCredentialCommandHandler(IUnitOfWork u, IMapper m, ICommonRequestService c, ILogger<TenantConfigurationHandlerBase> l, IIdEncoderService ids, IDeviceCommandSubmissionService commands, IEncryptionService encryption)
    : EmployeeDeviceEnrollmentHandlerBase(u, c, l, ids, commands, encryption), IRequestHandler<RemoveEmployeeDeviceCredentialCommand, ApiResponse<EmployeeDeviceEnrollmentResponseDTO>>
{
    public async Task<ApiResponse<EmployeeDeviceEnrollmentResponseDTO>> Handle(RemoveEmployeeDeviceCredentialCommand request, CancellationToken ct)
    {
        var dto = request.DTO ?? throw new ValidationErrorException("Credential type is required."); var (context, id) = await EnrollmentContext(dto.EnrollmentId, ct); var e = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetForUpdateAsync(context.TenantId, id, ct) ?? throw new NotFoundException("Employee device enrollment was not found."); await EnsureEmployeeDataAccessAsync(context, e.EmployeeId, EmployeeDataAccessRequirement.PersonalDetails, ct);
        var backup = dto.CredentialType switch { EmployeeDeviceCredentialType.Face => 50, EmployeeDeviceCredentialType.Card => 11, EmployeeDeviceCredentialType.Pin => 10, _ => throw new ValidationErrorException("Select a valid credential.") };
        await Commands.SubmitAsync(new DeviceCommandSubmission(e.TenantId, e.TenantDeviceId, DeviceCommands.DeleteUser, JsonSerializer.Serialize(new { cmd = DeviceCommands.DeleteUser, enrollid = long.Parse(e.EnrollId, CultureInfo.InvariantCulture), backupnum = backup }), context.LoggedInEmployeeId, ProtectPayload: true), ct);
        if (dto.CredentialType == EmployeeDeviceCredentialType.Face) { e.FaceImageHash = null; e.FaceDeploymentStatus = (short)DeviceCredentialDeploymentStatus.Removed; e.IsFaceEnrolled = false; }
        if (dto.CredentialType == EmployeeDeviceCredentialType.Pin) e.PinDeploymentStatus = (short)DeviceCredentialDeploymentStatus.Removed;
        if (dto.CredentialType == EmployeeDeviceCredentialType.Card) { if (e.TenantCardMaster is not null) e.TenantCardMaster.CardStatus = (short)TenantCardStatus.Available; e.TenantCardMasterId = null; e.CardDeploymentStatus = (short)DeviceCredentialDeploymentStatus.Removed; e.IsCardEnrolled = false; }
        e.UpdatedById = context.LoggedInEmployeeId; e.UpdatedDateTime = DateTime.UtcNow; await UnitOfWork.SaveChangesAsync(ct); var read = await UnitOfWork.EmployeeDeviceEnrollmentRepository.GetByIdAsync(context.TenantId, id, ct) ?? e; return ApiResponse<EmployeeDeviceEnrollmentResponseDTO>.Success(ToResponse(m, read, context.Claims.TenantEncriptionKey), "Credential removal has been queued for the device.");
    }
}
