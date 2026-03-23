using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.UpdateStatus.Handler
{


    public class UpdateEditableStatusCommand
       : IRequest<ApiResponse<bool>>
    {
        public UpdateEditStatusRequestDTO_ DTO { get; set; }

        public UpdateEditableStatusCommand(UpdateEditStatusRequestDTO_ dto)
        {
            DTO = dto;
        }
    }

    public class UpdateEditStatusCommandHandler
        : IRequestHandler<UpdateEditableStatusCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateEditStatusCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public UpdateEditStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateEditStatusCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config, ICommonRequestService commonRequestService,

            IEncryptionService encryptionService, IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }
        public async Task<ApiResponse<bool>> Handle(
    UpdateEditableStatusCommand request,
    CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("UpdateEditableStatus started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid employee.");

                // ===============================
                // 3️⃣ ENUM VALIDATION
                // ===============================
                if (!Enum.IsDefined(typeof(TabInfoType), request.DTO.TabInfoType))
                    throw new ValidationErrorException("Invalid section type.");

                // ===============================
                // 4️⃣ PERMISSION (CRITICAL 🚨)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update editable status.");

                // ===============================
                // 5️⃣ FETCH EMPLOYEE
                // ===============================
                var employee =
                    await _unitOfWork.Employees.GetByIdAsync(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.TenantId,
                        true);

                if (employee == null)
                    throw new ApiException("Employee not found.", 404);

                // ===============================
                // 6️⃣ UPDATE STATUS
                // ===============================
                var updated =
                    await _unitOfWork.Employees
                        .UpdateEditableStatusByEntityAsync(
                            request.DTO.TabInfoType,
                            request.DTO.Prop.EmployeeId,
                            validation.UserEmployeeId,
                            request.DTO.IsEditable,
                            ct);

                if (!updated)
                {
                    _logger.LogWarning(
                        "Editable update failed | EmpId={EmpId} | Tab={Tab}",
                        request.DTO.Prop.EmployeeId,
                        request.DTO.TabInfoType);

                    throw new ApiException("Editable update failed.", 500);
                }

                _logger.LogInformation("UpdateEditableStatus success");

                return ApiResponse<bool>.Success(
                    true,
                    "Editable update completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Editable update error");

                throw; // 🚨 MUST
            }
        }

    }
}
