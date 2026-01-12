using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
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
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    public class GetAllCategoryCommand : IRequest<ApiResponse<List<GetCategoryResponseDTO>>>
    {
        public GetCategoryReqestDTO DTO { get; set; }

        public GetAllCategoryCommand(GetCategoryReqestDTO dto)
        {
            DTO = dto;
        }
    }
    /// <summary>
    /// Handles fetching all Asset Categories for a given tenant.
    /// </summary>
    public class GetAllCategoryCommandHandler
        : IRequestHandler<GetAllCategoryCommand, ApiResponse<List<GetCategoryResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCategoryCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncryptionService _encryptionService;


        public GetAllCategoryCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetAllCategoryCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService ,
            IHttpContextAccessor _httpContextAccessor, IEmailService emailService
)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _configuration = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _emailService = emailService;
        }

        /// <summary>
        /// Handles the GetAllCategoryCommand request to retrieve all categories.
        /// </summary>
        public async Task<ApiResponse<List<GetCategoryResponseDTO>>> Handle(GetAllCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Categories ");

                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetCategoryResponseDTO>>.Fail(validation.ErrorMessage);

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



                // ✅ Fetch data from repository
                var categoryEntities = await _unitOfWork.AssetCategoryRepository.GetAllAsync(request.DTO);

                if (categoryEntities == null || categoryEntities.Count == 0)
                {
                    _logger.LogWarning("No Asset Categories found for TenantId: {TenantId}", request.DTO.Prop.TenantId);
                    return new ApiResponse<List<GetCategoryResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Asset Categories found.",
                        Data = new List<GetCategoryResponseDTO>()
                    };
                }

                // ✅ Map entities → DTOs
                var responseDTOs = _mapper.Map<List<GetCategoryResponseDTO>>(categoryEntities);

                _logger.LogInformation("Successfully retrieved {Count} Asset Categories for TenantId: {TenantId}",
                    responseDTOs.Count, request.DTO.Prop.TenantId);

                // ✅ Return formatted API response
                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Asset Categories fetched successfully.",
                    Data = responseDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Asset Categories for TenantId: {TenantId}", request.DTO.Prop.TenantId);
                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching asset categories.",
                    Data = null
                };
            }
        }
    }
}
