// ================================================================
// Purpose : Host-only initial physical-device bootstrap. Tenant runtime
//           configuration is intentionally handled separately after assignment.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IDeviceCommunication;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.HostDeviceCmd.Handlers;

/// <summary>Issues a temporary initial HTTPS URL for an unassigned physical device.</summary>
public sealed class IssueInitialDeviceBootstrapCommand(IssueInitialDeviceBootstrapRequestDTO dto)
    : IRequest<ApiResponse<InitialDeviceBootstrapResponseDTO>>
{
    public IssueInitialDeviceBootstrapRequestDTO DTO { get; } = dto;
}

/// <summary>Enforces Host runtime permission before creating a bootstrap URL.</summary>
public sealed class IssueInitialDeviceBootstrapCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IDeviceInitialProvisioningService initialProvisioningService)
    : IRequestHandler<IssueInitialDeviceBootstrapCommand, ApiResponse<InitialDeviceBootstrapResponseDTO>>
{
    private const string HostInitialDeviceConfigurationModuleCode = "HOST_INITIAL_DEVICE_CONFIGURATION";

    /// <inheritdoc />
    public async Task<ApiResponse<InitialDeviceBootstrapResponseDTO>> Handle(
        IssueInitialDeviceBootstrapCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException("The initial device bootstrap request is required.");
        if (dto.DeviceMasterId <= 0)
        {
            throw new ValidationErrorException("A valid physical device is required.");
        }

        if (dto.ModuleId <= 0 || dto.OperationId <= 0 ||
            !string.Equals(
                await commonRequestService.GetModuleCodeAsync(dto.ModuleId),
                HostInitialDeviceConfigurationModuleCode,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException("The Host initial device configuration permission is required.");
        }

        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            commonRequestService,
            unitOfWork.StoreProcedureRepository,
            dto.ModuleId,
            dto.OperationId,
            cancellationToken);

        var issue = await initialProvisioningService.IssueAsync(
            dto.DeviceMasterId,
            dto.LifetimeMinutes,
            hostContext.HostUserId,
            cancellationToken);

        return ApiResponse<InitialDeviceBootstrapResponseDTO>.Success(
            new InitialDeviceBootstrapResponseDTO
            {
                DeviceSerialNumber = issue.DeviceSerialNumber,
                InitialGatewayUrl = issue.InitialGatewayUrl,
                HeartbeatIntervalSeconds = issue.HeartbeatIntervalSeconds,
                ExpiresDateTime = issue.ExpiresDateTime
            },
            "Initial device gateway URL generated. It will not be shown again.");
    }
}
