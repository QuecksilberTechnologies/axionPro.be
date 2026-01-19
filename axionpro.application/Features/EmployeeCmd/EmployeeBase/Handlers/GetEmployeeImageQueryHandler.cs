using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
 
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class GetEmployeeImageQuery : IRequest<ApiResponse<GetEmployeeImageReponseDTO>>
    {
        public GetEmployeeImageRequestDTO DTO { get; }

        public GetEmployeeImageQuery(GetEmployeeImageRequestDTO dTO)
        {
            DTO = dTO;
        }
    }


    public class GetEmployeeImageQueryHandler
     : IRequestHandler<GetEmployeeImageQuery, ApiResponse<GetEmployeeImageReponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmployeeImageQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _config;
        private readonly IIdEncoderService _idEncoderService;

        public GetEmployeeImageQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetEmployeeImageQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService,
            IConfiguration config,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
            _config = config;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<GetEmployeeImageReponseDTO>> Handle(
            GetEmployeeImageQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<GetEmployeeImageReponseDTO>
                        .Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;             
                request.DTO.Prop.TenantId = validation.TenantId;
               
                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);
                // ===============================
                // 2️⃣ PERMISSION (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("ViewEmployeeImage"))
                //     return ApiResponse<GetEmployeeImageReponseDTO>
                //         .Fail("Permission denied.");

                // ===============================
                // 3️⃣ FETCH SINGLE IMAGE
                // ===============================
                var image =
                    await _unitOfWork.Employees.GetImage(request.DTO);

                if (image == null)
                {
                    _logger.LogInformation(
                        "No primary image found | EmployeeId={EmpId}",
                        request.DTO.Prop.EmployeeId);

                    return ApiResponse<GetEmployeeImageReponseDTO>
                        .Fail("Employee image not found.");
                }

                // ===============================
                // 4️⃣ BUILD FULL IMAGE URL (OPTIONAL)
                // ===============================
                string baseUrl =
                    _config["FileSettings:BaseUrl"] ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(image.FilePath))
                    image.FilePath = $"{baseUrl}{image.FilePath}";
                image.EmployeeId = request.DTO.EmployeeId;
                // ===============================
                // 5️⃣ SUCCESS
                // ===============================
                return ApiResponse<GetEmployeeImageReponseDTO>
                    .Success(image, "Employee image fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching employee image | EmpId={EmpId}",
                    request.DTO?.Prop?.EmployeeId);

                return ApiResponse<GetEmployeeImageReponseDTO>
                    .Fail("Unexpected error while fetching employee image.");
            }
        }
    }

}
