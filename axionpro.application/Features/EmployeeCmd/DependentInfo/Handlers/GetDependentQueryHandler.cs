using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Pagination;
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

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    public class GetDependentInfoQuery : IRequest<ApiResponse<List<GetDependentResponseDTO>>>
    {
        public GetDependentRequestDTO DTO { get; set; }

        public GetDependentInfoQuery(GetDependentRequestDTO dto)
        {
            DTO = dto;
        }

    }
    public class GetDependentInfoQueryHandler
   : IRequestHandler<GetDependentInfoQuery, ApiResponse<List<GetDependentResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDependentInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _configuration;

        public GetDependentInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetDependentInfoQueryHandler> logger,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<List<GetDependentResponseDTO>>> Handle(
            GetDependentInfoQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: COMMON VALIDATION (SAME AS CONTACT)
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetDependentResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // 🔓 STEP 2: Assign decoded props
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(
                        validation.RoleId);

                if (!permissions.Contains("ViewDependentInfo"))
                {
                    // optional strict check
                    // return ApiResponse<List<GetDependentResponseDTO>>
                    //     .Fail("You do not have permission to view dependent info.");
                }

                // 📦 STEP 4: Repository call
                var result =
                    await _unitOfWork.EmployeeDependentRepository
                        .GetInfo(request.DTO);

                if (result == null || !result.Items.Any())
                    return ApiResponse<List<GetDependentResponseDTO>>
                        .Fail("No dependent info found.");

                // 🔐 STEP 5: Projection + Encryption + FilePath + Completion %
                var responseDTO =
                    ProjectionHelper.ToGetDependentResponseDTOs(
                        result.Items,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey,
                        _configuration
                    );

                await _unitOfWork.CommitTransactionAsync();

                // 📤 STEP 6: Response (same style as Contact)
                return ApiResponse<List<GetDependentResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: responseDTO,
                        Message: "Dependent info retrieved successfully.",
                        PageNumber: result.PageNumber,
                        PageSize: result.PageSize,
                        TotalRecords: result.TotalCount,
                        TotalPages: result.TotalPages,
                        CompletionPercentage: result.Items.Any()
                            ? Math.Round(responseDTO.Average(x => x.CompletionPercentage), 0)
                            : 0,
                        HasUploadedAll: null
                    );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error occurred while fetching Dependent info for EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                return ApiResponse<List<GetDependentResponseDTO>>
                    .Fail("Failed to fetch dependent info.",
                          new List<string> { ex.Message });
            }
        }
    }


}
