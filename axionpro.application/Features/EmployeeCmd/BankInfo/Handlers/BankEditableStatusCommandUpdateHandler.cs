using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
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

    public class BankEditableStatusCommandUpdateHandler
        : IRequestHandler<UpdateEditableStatusCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BankEditableStatusCommandUpdateHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public BankEditableStatusCommandUpdateHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<BankEditableStatusCommandUpdateHandler> logger,
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
                bool updateResult = await _unitOfWork.EmployeeBankRepository.UpdateEditStatus(
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
