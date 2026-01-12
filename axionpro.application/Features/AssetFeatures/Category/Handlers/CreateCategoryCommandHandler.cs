using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    public class AddCategoryCommand
        : IRequest<ApiResponse<List<GetCategoryResponseDTO>>>
    {
        public AddCategoryReqestDTO DTO { get; set; }

        public AddCategoryCommand(AddCategoryReqestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles creation of Asset Category
    /// </summary>
    public class CreateCategoryCommandHandler
        : IRequestHandler<AddCategoryCommand, ApiResponse<List<GetCategoryResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public CreateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateCategoryCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<List<GetCategoryResponseDTO>>> Handle(
            AddCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating Asset Category");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetCategoryResponseDTO>>
                        .Fail(validation.ErrorMessage);

                if (validation.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid TenantId provided in AddAsync: {TenantId}", (request.DTO.Prop.TenantId));
                }
                 request.DTO.Prop.TenantId = validation.TenantId;
                

                if (string.IsNullOrWhiteSpace(request.DTO.CategoryName))
                {
                    _logger.LogWarning("CategoryName is missing in AddAsync request for TenantId: {TenantId}", request.DTO.Prop.TenantId);
                    throw new ArgumentException("CategoryName cannot be null or empty.", nameof(request.DTO.CategoryName));
                }
               
 

                // ===============================
                // 2️⃣ PERMISSION CHECK (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("AddAssetCategory"))
                // {
                //     return ApiResponse<List<GetCategoryResponseDTO>>
                //         .Fail("You do not have permission to add asset category.");
                // }

                // ===============================
                // 3️⃣ CALL REPOSITORY
                // ===============================
                var categories =
                    await _unitOfWork.AssetCategoryRepository
                        .AddAsync(request.DTO);
                 

                if (categories == null || categories.Count == 0)
                {
                    _logger.LogWarning(
                        "Asset Category created but no data returned for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return new ApiResponse<List<GetCategoryResponseDTO>>
                    {
                        IsSucceeded = true,
                        Message = "Asset category created, but no categories found.",
                        Data = new List<GetCategoryResponseDTO>()
                    };
                }

                _logger.LogInformation(
                    "Asset Category created successfully for TenantId: {TenantId}",
                    request.DTO.Prop.TenantId);

                // ===============================
                // 4️⃣ RETURN RESPONSE
                // ===============================
                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Asset category created successfully.",
                    Data = categories
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while creating Asset Category for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while creating asset category.",
                    Data = null
                };
            }
        }
    }


}
