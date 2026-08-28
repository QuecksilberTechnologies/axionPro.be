// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Reports future Tenant deletion dependency groups without changing data.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Queries;

/// <summary>Represents the Host-side read-only request for Tenant deletion dependency information.</summary>
public sealed record GetTenantDeleteDependencyInfoQuery(string EncryptedTenantId, PermissionRequestDTO PermissionRequest)
    : IRequest<ApiResponse<TenantDeleteDependencyInfoResponseDTO>>;

/// <summary>Validates a Host request and reports the transactional groups reserved for a future Tenant deletion cascade.</summary>
public sealed class GetTenantDeleteDependencyInfoQueryHandler
    : IRequestHandler<GetTenantDeleteDependencyInfoQuery, ApiResponse<TenantDeleteDependencyInfoResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly IIdEncoderService _idEncoderService;

    public GetTenantDeleteDependencyInfoQueryHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService,
        IIdEncoderService idEncoderService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _idEncoderService = idEncoderService;
    }

    public async Task<ApiResponse<TenantDeleteDependencyInfoResponseDTO>> Handle(
        GetTenantDeleteDependencyInfoQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.PermissionRequest);

        var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
            _commonRequestService,
            _unitOfWork.StoreProcedureRepository,
            request.PermissionRequest.ModuleId,
            request.PermissionRequest.OperationId,
            cancellationToken);
        var tenantId = HostTenantIdentifierProtector.Decrypt(
            request.EncryptedTenantId,
            hostContext.TenantEncryptionKey,
            _idEncoderService);
        var tenant = await _unitOfWork.TenantRepository
            .GetHostManagedTenantByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            throw new NotFoundException(AppConstants.ErrorMessages.ResourceNotFound);
        }

        return ApiResponse<TenantDeleteDependencyInfoResponseDTO>.Success(
            new TenantDeleteDependencyInfoResponseDTO
            {
                TenantId = HostTenantIdentifierProtector.Encrypt(tenant.Id, hostContext.TenantEncryptionKey, _idEncoderService),
                Message = "Tenant deletion is not executed. A future cascade must process the listed transactional data groups.",
                TransactionalDataGroups = new List<string>
                {
                    "Leave and leave-policy records",
                    "Attendance and work-arrangement records",
                    "Payroll and salary records",
                    "Tickets and ticket-history records",
                    "Assets and asset-assignment records",
                    "Employee, login, role, and permission records",
                    "Tenant configuration, modules, operations, and subscription records"
                }
            },
            "Tenant deletion dependency information retrieved successfully.");
    }
}
