using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{


    public class UpdateEditableStatusCommand
       : IRequest<ApiResponse<bool>>
    {
        public UpdateEditStatusRequestDTO DTO { get; set; }

        public UpdateEditableStatusCommand(UpdateEditStatusRequestDTO dto)
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


        public async Task<ApiResponse<bool>> Handle(UpdateEditableStatusCommand request, CancellationToken ct)
        {
            try
            {
                 //    ===================================================== */
                var validation =
                    await _commonRequestService.ValidateRequestAsync(
                        request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                /* =====================================================
                   2️⃣ FETCH EXISTING EMPLOYEE
                   ===================================================== */
                var employee = await _unitOfWork.Employees.GetByIdAsync(
                    request.DTO.Prop.EmployeeId,
                    request.DTO.Prop.TenantId,
                    true);

                if (employee == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Employee not found.");
                }

                // 🧩 STEP 4: UPDATE EDITABLE STATUS
                bool updateResult = await _unitOfWork.Employees.UpdateEditStatus(
                    request.DTO.Prop.EmployeeId,
                    request.DTO.Prop.UserEmployeeId,
                    request.DTO.IsEditable);
                if (!updateResult)
                {
                    _logger.LogWarning("❌ Failed to update editable status for EmployeeId: {EmployeeId}", request.DTO.Prop.EmployeeId);
                }


            return ApiResponse<bool>.Success(true, "editable update completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "editable update error");
                return ApiResponse<bool>.Fail("Unexpected error occurred.");
            }
        }

    }
}
