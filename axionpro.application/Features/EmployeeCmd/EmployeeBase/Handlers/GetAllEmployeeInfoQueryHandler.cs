using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
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
    public class GetAllEmployeeInfoQuery : IRequest<ApiResponse<List<GetAllEmployeeInfoResponseDTO>>>
    {
        public GetAllEmployeeInfoRequestDTO DTO { get; }

        public GetAllEmployeeInfoQuery(GetAllEmployeeInfoRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetAllEmployeeInfoQueryHandler : IRequestHandler<GetAllEmployeeInfoQuery, ApiResponse<List<GetAllEmployeeInfoResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetAllEmployeeInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllEmployeeInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetAllEmployeeInfoQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetAllEmployeeInfoResponseDTO>>> Handle(GetAllEmployeeInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;


                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                PagedResponseDTO<GetAllEmployeeInfoResponseDTO> responseDTO = await _unitOfWork.Employees.GetAllInfo(request.DTO);
                if (responseDTO == null || !responseDTO.Items.Any())
                {
                    _logger.LogInformation("No Base Employee info found for EmployeeId: {EmpId}", request.DTO.Prop.UserEmployeeId );
                    return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("No Base Employee info found.");
                }

                var resultList = ProjectionHelper.ToGetAllEmployeeInfoResponseDTOs(responseDTO, _idEncoderService, validation.Claims.TenantEncriptionKey, _config);

                return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.SuccessPaginatedPercentage(
                    Data: resultList,
                    Message: "Base Employee info retrieved successfully.",
                    PageNumber: responseDTO.PageNumber,
                    PageSize: responseDTO.PageSize,
                    TotalRecords: responseDTO.TotalCount,
                    TotalPages: responseDTO.TotalPages,
                    CompletionPercentage: responseDTO.CompletionPercentage,
                    HasUploadedAll: responseDTO.HasUploadedAll
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Base Employee info for EmployeeId: {EmployeeId}", request.DTO?.UserEmployeeId);
                return ApiResponse<List<GetAllEmployeeInfoResponseDTO>>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }

    }
}

