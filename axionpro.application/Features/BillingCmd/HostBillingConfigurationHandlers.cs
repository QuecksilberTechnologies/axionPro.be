using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.Billing;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.BillingCmd;

public sealed record GetHostBillingConfigurationQuery(PermissionRequestDTO PermissionRequest)
    : IRequest<ApiResponse<HostBillingConfigurationResponseDTO>>;

public sealed record UpdateHostBillingConfigurationCommand(HostBillingConfigurationRequestDTO DTO)
    : IRequest<ApiResponse<HostBillingConfigurationResponseDTO>>;

internal static class HostBillingConfigurationAuthorization
{
    public static async Task<long> ValidateAsync(
        PermissionRequestDTO permission,
        ICommonRequestService commonRequestService,
        IStoreProcedureRepository storeProcedureRepository,
        CancellationToken cancellationToken,
        string expectedModuleCode = BillingConstants.HostBillingConfigurationModuleCode,
        string? expectedOperationName = null)
    {
        var moduleCode = await commonRequestService.GetModuleCodeAsync(permission.ModuleId);
        if (!string.Equals(moduleCode, expectedModuleCode, StringComparison.Ordinal))
        {
            throw new ForbiddenAccessException($"Use the {expectedModuleCode} module permission.");
        }

        if (expectedOperationName is not null)
        {
            var operationName = await commonRequestService.GetActiveOperationNameAsync(permission.OperationId);
            if (!string.Equals(operationName, expectedOperationName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenAccessException($"Use the {expectedOperationName} operation permission.");
            }
        }

        var context = await HostRuntimePermissionValidator.ValidateAsync(
            commonRequestService,
            storeProcedureRepository,
            permission.ModuleId,
            permission.OperationId,
            cancellationToken);
        return context.HostUserId;
    }
}

public sealed class GetHostBillingConfigurationQueryHandler(
    IHostBillingConfigurationRepository repository,
    ICommonRequestService commonRequestService,
    IStoreProcedureRepository storeProcedureRepository)
    : IRequestHandler<GetHostBillingConfigurationQuery, ApiResponse<HostBillingConfigurationResponseDTO>>
{
    public async Task<ApiResponse<HostBillingConfigurationResponseDTO>> Handle(
        GetHostBillingConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        await HostBillingConfigurationAuthorization.ValidateAsync(
            request.PermissionRequest, commonRequestService, storeProcedureRepository, cancellationToken,
            expectedOperationName: "View");
        var configuration = await repository.GetAsync(cancellationToken)
            ?? throw new NotFoundException("Billing configuration was not found.");
        return ApiResponse<HostBillingConfigurationResponseDTO>.Success(configuration, "Billing configuration retrieved successfully.");
    }
}

public sealed class UpdateHostBillingConfigurationCommandHandler(
    IHostBillingConfigurationRepository repository,
    ICommonRequestService commonRequestService,
    IStoreProcedureRepository storeProcedureRepository)
    : IRequestHandler<UpdateHostBillingConfigurationCommand, ApiResponse<HostBillingConfigurationResponseDTO>>
{
    public async Task<ApiResponse<HostBillingConfigurationResponseDTO>> Handle(
        UpdateHostBillingConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        var hostUserId = await HostBillingConfigurationAuthorization.ValidateAsync(
            request.DTO.PermissionRequest, commonRequestService, storeProcedureRepository, cancellationToken,
            expectedOperationName: "Update");
        var configuration = await repository.UpdateAsync(request.DTO, hostUserId, cancellationToken);
        return ApiResponse<HostBillingConfigurationResponseDTO>.Success(configuration, "Billing configuration updated successfully.");
    }
}
