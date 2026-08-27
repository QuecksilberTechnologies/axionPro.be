// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves an employee-code pattern while preserving Tenant access and protecting the Host Tenant boundary.
// ================================================================

using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Configuration.EmployeeCodeCmd.Handlers
{
    // ======================= QUERY ============================
    public class GetEmployeeCodePatternQuery
        : IRequest<ApiResponse<object>>
    {
        public EmployeeCodePatternRequestDTO DTO { get; }

        public GetEmployeeCodePatternQuery(EmployeeCodePatternRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================= HANDLER ============================
    public class GetEmployeeCodePatternQueryHandler
        : IRequestHandler<GetEmployeeCodePatternQuery, ApiResponse<object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ILogger<GetEmployeeCodePatternQueryHandler> _logger;

        public GetEmployeeCodePatternQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService,
            ILogger<GetEmployeeCodePatternQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _logger = logger;
        }

        public async Task<ApiResponse<object>> Handle(GetEmployeeCodePatternQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "GetEmployeeCodePattern started.");

                // ===============================
                // 1️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                {
                    throw new ValidationErrorException(
                        "Invalid request data.");
                }

                // ===============================
                // 2️⃣ TENANT OR HOST VALIDATION
                // ===============================
                var tenantValidation =
                    await _commonRequestService.ValidateTenantUserRequestAsync();

                long targetTenantId;
                string? hostTenantEncryptionKey = null;

                if (tenantValidation.Success)
                {
                    // Tenant users can access only their own tenant.
                    targetTenantId = tenantValidation.TenantId;

                    // ===============================
                    // 2️⃣ PERMISSION CHECK (RBAC)
                    // ===============================
                    //var hasAccess = await _permissionService.HasAccessAsync(
                    //    validation.RoleId,
                    //    Modules.Employee,
                    //    Operations.View);

                    //if (!hasAccess)
                    //    throw new UnauthorizedAccessException("Access denied.");
                }
                else
                {
                    var hostContext = await HostRuntimePermissionValidator.ValidateAsync(
                        _commonRequestService,
                        _unitOfWork.StoreProcedureRepository,
                        request.DTO.ModuleId,
                        request.DTO.OperationId,
                        cancellationToken);

                    targetTenantId = HostTenantIdentifierProtector.Decrypt(
                        request.DTO.TenantId,
                        hostContext.TenantEncryptionKey,
                        _idEncoderService);
                    hostTenantEncryptionKey = hostContext.TenantEncryptionKey;
                }

                // ===============================
                // 3️⃣ FETCH DATA
                // ===============================
                var pattern = await _unitOfWork
                    .TenantEmployeeCodePatternRepository
                    .GetTenantEmployeeCodePatternAsync(
                        targetTenantId,
                        request.DTO.IsActive);

                if (pattern == null)
                {
                    _logger.LogInformation(
                        "No employee code pattern found.");

                    return ApiResponse<object>
                        .Success(null, "No pattern found.");
                }

                _logger.LogInformation(
                    "Employee code pattern retrieved.");

                object response = hostTenantEncryptionKey is null
                    ? pattern
                    : MapHostResponse(pattern, hostTenantEncryptionKey);

                return ApiResponse<object>
                    .Success(
                        response,
                        "Pattern fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "GetEmployeeCodePattern failed.");

                throw;
            }
        }

        private HostEmployeeCodePatternResponseDTO MapHostResponse(
            GetEmployeeCodePatternResponseDTO pattern,
            string tenantEncryptionKey)
        {
            return new HostEmployeeCodePatternResponseDTO
            {
                Id = pattern.Id,
                TenantId = HostTenantIdentifierProtector.Encrypt(
                    pattern.TenantId,
                    tenantEncryptionKey,
                    _idEncoderService),
                Prefix = pattern.Prefix,
                IncludeYear = pattern.IncludeYear,
                IncludeMonth = pattern.IncludeMonth,
                IncludeDepartment = pattern.IncludeDepartment,
                Separator = pattern.Separator,
                RunningNumberLength = pattern.RunningNumberLength,
                LastUsedNumber = pattern.LastUsedNumber,
                IsActive = pattern.IsActive,
                AddedById = pattern.AddedById,
                AddedDateTime = pattern.AddedDateTime,
                UpdatedById = pattern.UpdatedById,
                UpdatedDateTime = pattern.UpdatedDateTime
            };
        }

    }
}

