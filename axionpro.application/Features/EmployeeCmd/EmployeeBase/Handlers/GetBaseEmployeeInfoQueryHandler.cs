using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class GetBaseEmployeeInfoQuery : IRequest<ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        public GetBaseEmployeeRequestDTO DTO { get; }

        public GetBaseEmployeeInfoQuery(GetBaseEmployeeRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetBaseEmployeeInfoQueryHandler : IRequestHandler<GetBaseEmployeeInfoQuery, ApiResponse<List<GetBaseEmployeeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetBaseEmployeeInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public GetBaseEmployeeInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetBaseEmployeeInfoQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetBaseEmployeeResponseDTO>>> Handle(GetBaseEmployeeInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(request.DTO.EmployeeId, validation.Claims.TenantEncriptionKey, _idEncoderService );

                if (request.DTO.Prop.EmployeeId<=0)
                {
                    _logger.LogInformation("Invalid EmployeeId provided in the request.");
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("Invalid EmployeeId provided.");
                }
                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data
                // 🧩 STEP 4: Call Repository to get data
                var responseDTO = await _unitOfWork.Employees.GetInfo(request.DTO);
                if (responseDTO == null || !responseDTO.Items.Any())
                {
                    _logger.LogInformation("No Base Employee info found for EmployeeId: {EmpId}", request.DTO.Prop.EmployeeId);
                    return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("No Base Employee info found.");
                }

              
                var resultList = ProjectionHelper.ToGetBaseInfoListResponseDTOs(responseDTO.Items, _idEncoderService, validation.Claims.TenantEncriptionKey);

                // 🧩 STEP 8: Construct success response with pagination
                return ApiResponse<List<GetBaseEmployeeResponseDTO>>.SuccessPaginatedPercentage(
                    Data: resultList,
                    Message: "Base Employee info retrieved successfully.",
                    PageNumber: responseDTO.PageNumber,
                    PageSize: responseDTO.PageSize,
                    TotalRecords: responseDTO.TotalCount,
                    TotalPages: responseDTO.TotalPages,
                      HasUploadedAll: null,
                    CompletionPercentage: null


                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error while fetching Base Employee info for EmployeeId: {EmployeeId}", request.DTO?.UserEmployeeId);
                return ApiResponse<List<GetBaseEmployeeResponseDTO>>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }

    }
}
