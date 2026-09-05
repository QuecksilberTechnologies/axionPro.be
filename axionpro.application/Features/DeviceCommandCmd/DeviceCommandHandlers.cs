// ================================================================
// Purpose : Handles authenticated MQTT command submission. The durable storage
//           service resolves serial, Tenant, and location from TenantDevice.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IDeviceCommunication;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DeviceCommandCmd;

/// <summary>Queues a supported MQTT command after authorization has completed.</summary>
public sealed class SubmitDeviceCommand(SubmitDeviceCommandRequestDTO dto)
    : IRequest<ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public SubmitDeviceCommandRequestDTO DTO { get; } = dto;
}

/// <summary>Submits an authorized command into the per-device sequential pipeline.</summary>
public sealed class SubmitDeviceCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<SubmitDeviceCommand, ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    /// <inheritdoc />
    public async Task<ApiResponse<DeviceCommandSubmissionResponseDTO>> Handle(
        SubmitDeviceCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.TenantDeviceId <= 0)
        {
            throw new ArgumentException("A valid Tenant device identifier is required.");
        }

        var definition = DeviceProtocolCommandCatalog.GetRequired(request.DTO.CommandName);
        var scope = await ResolveTenantScopeAsync(request.DTO, cancellationToken);
        var payload = DeviceProtocolCommandCatalog.ValidatePayload(definition.Name, request.DTO.Payload);
        var result = await deviceCommandSubmissionService.SubmitAsync(
            new DeviceCommandSubmission(
                scope.TenantId,
                request.DTO.TenantDeviceId,
                definition.Name,
                payload,
                scope.ActorId),
            cancellationToken);

        return ApiResponse<DeviceCommandSubmissionResponseDTO>.Success(
            new DeviceCommandSubmissionResponseDTO
            {
                DeviceCommandId = result.DeviceCommandId,
                InternalTrackingId = result.InternalTrackingId,
                DeviceSerialNumber = result.DeviceSerialNumber,
                Status = result.Status.ToString()
            },
            "Device command queued successfully.");
    }
}
