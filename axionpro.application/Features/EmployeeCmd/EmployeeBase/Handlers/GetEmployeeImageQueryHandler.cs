using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;

using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        private readonly IFileStorageService _fileStorageService;
        public GetEmployeeImageQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetEmployeeImageQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService,
            IConfiguration config,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
            _config = config;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<GetEmployeeImageReponseDTO>> Handle(
     GetEmployeeImageQuery request,
     CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validation
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<GetEmployeeImageReponseDTO>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                // 2️⃣ Fetch image
                var image = await _unitOfWork.Employees.GetImage(request.DTO);

                if (image == null)
                {
                    _logger.LogInformation(
                        "No primary image found | EmployeeId={EmpId}",
                        request.DTO.Prop.EmployeeId);

                    return ApiResponse<GetEmployeeImageReponseDTO>
                        .Fail("Employee image not found.");
                }

                // 3️⃣ 🔥 Convert S3 key → Signed URL
                if (!string.IsNullOrWhiteSpace(image.FilePath))
                {
                    image.FilePath =  _fileStorageService.GetFileUrl(image.FilePath);
                }

                image.EmployeeId = request.DTO.EmployeeId;

                return ApiResponse<GetEmployeeImageReponseDTO>
                    .Success(image, "Employee image fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching employee image");

                return ApiResponse<GetEmployeeImageReponseDTO>
                    .Fail("Unexpected error while fetching employee image.");
            }
        }
    }

}
