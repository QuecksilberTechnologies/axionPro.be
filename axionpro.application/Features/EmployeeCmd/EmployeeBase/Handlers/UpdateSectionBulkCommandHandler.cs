using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.BaseEmployee;
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


    public class UpdateSectionBulkCommand
       : IRequest<ApiResponse<bool>>
    {
        public UpdateEmployeeSectionStatusRequestDTO DTO { get; set; }

        public UpdateSectionBulkCommand(UpdateEmployeeSectionStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateSectionBulkCommandHandler
        : IRequestHandler<UpdateSectionBulkCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateSectionBulkCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;


        public UpdateSectionBulkCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateSectionBulkCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)
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


        public async Task<ApiResponse<bool>> Handle(UpdateSectionBulkCommand request, CancellationToken ct)
        {
            try
            { 
                
                var validation =   await _commonRequestService.ValidateRequestAsync(
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

                if (request.DTO.Sections == null)
                    return ApiResponse<bool>.Fail("Invalid Selection Id.");

                
                // ------------------------------------
                // STEP 4: LOOP ALL SECTIONS
                // ------------------------------------
                foreach (var section in request.DTO.Sections)
                {
                    // Validate section fields
                    if (string.IsNullOrWhiteSpace(section.SectionName))
                        return ApiResponse<bool>.Fail("Section name missing.");

                   

                                  

                    // ------------------------------------
                    // UPDATE VIA UOW (YOUR SAME CALL STYLE)
                    // ------------------------------------
                    bool isUpdated = await _unitOfWork.Employees.UpdateVerifyEditStatusAsync(
                        section.SectionName?.Trim().ToLower(),
                        request.DTO.Prop.EmployeeId,
                        section.IsVerified,
                        section.IsEditAllowed,
                        true,                    // isActive default
                         request.DTO.Prop.UserEmployeeId          // admin who verified
                    );

                    if (!isUpdated)
                    {
                        _logger.LogWarning(
                            "Update failed → Section={Sec}, EmpId={EmpId}",
                            section.SectionName,
                            request.DTO.Prop.EmployeeId
                        );
                    }
                }

                return ApiResponse<bool>.Success(true, "Section update completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update error");
                return ApiResponse<bool>.Fail("Unexpected error occurred.");
            }
        }

    }
}
