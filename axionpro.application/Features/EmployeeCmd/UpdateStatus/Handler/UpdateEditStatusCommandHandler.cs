using AutoMapper;
using axionpro.application.Common.Enums;
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
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    return ApiResponse<bool>.Fail("Invalid employee.");

                if (!Enum.IsDefined(typeof(TabInfoType), request.DTO.TabInfoType))
                    return ApiResponse<bool>.Fail("Invalid section type.");

                var employee = await _unitOfWork.Employees.GetByIdAsync(
                    request.DTO.Prop.EmployeeId,
                    request.DTO.Prop.TenantId,
                    true);

                if (employee == null)
                    return ApiResponse<bool>.Fail("Employee not found.");

                bool updated = await _unitOfWork.Employees
                    .UpdateEditableStatusByEntityAsync(
                        request.DTO.TabInfoType,
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.UserEmployeeId,
                        request.DTO.IsEditable,
                        ct);

                if (!updated)
                {
                    _logger.LogWarning(
                        "Editable status update failed | EmpId={EmpId} | Tab={Tab}",
                        request.DTO.Prop.EmployeeId,
                        request.DTO.TabInfoType);

                    return ApiResponse<bool>.Fail("Editable update failed.");
                }

                return ApiResponse<bool>.Success(true, "Editable update completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Editable update error");
                return ApiResponse<bool>.Fail("Unexpected error occurred.");
            }
        }

    }
}
