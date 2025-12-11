using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
 
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class GetRoleQuery : IRequest<ApiResponse<List<GetRoleResponseDTO>>>
    {
        public GetRoleRequestDTO DTO { get; set; }

        public GetRoleQuery(GetRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, ApiResponse<List<GetRoleResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetRoleQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
       


        public GetRoleQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetRoleQueryHandler> logger,
            ITokenService tokenService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _config = config;
            _encryptionService = encryptionService;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetRoleResponseDTO>>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //  COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;


                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);


                if (!permissions.Contains("AddBankInfo"))
                {
                    // await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }



                var responseDTO = await _unitOfWork.RoleRepository.GetAsync(request.DTO );

                if (responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogInformation("⚠️ No roles found for TenantId: {TenantId}", request.DTO.Prop.TenantId);
                    return new ApiResponse<List<GetRoleResponseDTO>>
                    {
                        IsSucceeded = true,
                        Message = "No roles found.",
                        Data = new List<GetRoleResponseDTO>(),
                        PageNumber = request.DTO.PageNumber,
                        PageSize = request.DTO.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0
                    };
                }

              //  var encryptedList = ProjectionHelper.ToGetRoleResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);


                // 🧩 STEP 7: Success response
                _logger.LogInformation("✅ {Count} roles retrieved successfully for TenantId: {TenantId}",
                    responseDTO.TotalCount, request.DTO.Prop.TenantId);

                return new ApiResponse<List<GetRoleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Roles retrieved successfully.",
                    Data = responseDTO.Items,
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages
                };
            }
            catch (Exception ex)
            {
                // 🧩 STEP 8: Error Handling
                _logger.LogError(ex, "❌ Error occurred while retrieving roles.");
                return ApiResponse<List<GetRoleResponseDTO>>.Fail("Error occurred while retrieving roles.");
            }
        }
    }
}
