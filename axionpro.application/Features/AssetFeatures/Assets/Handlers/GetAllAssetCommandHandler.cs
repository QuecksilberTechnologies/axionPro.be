using AutoMapper;
using axionpro.application.DTOS.AssetDTO.asset;
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

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class GetAllAssetCommand
        : IRequest<ApiResponse<List<GetAssetResponseDTO>>>
    {
        public GetAssetRequestDTO DTO { get; set; }

        public GetAllAssetCommand(GetAssetRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles fetching all Assets for a given tenant.
    /// </summary>
    public class GetAllAssetCommandHandler
        : IRequestHandler<GetAllAssetCommand, ApiResponse<List<GetAssetResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllAssetCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public GetAllAssetCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllAssetCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<List<GetAssetResponseDTO>>> Handle(
            GetAllAssetCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Assets");

                // ===============================
                // 1️⃣ COMMON VALIDATION (MANDATORY)
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetAssetResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ PERMISSION CHECK (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("ViewAsset"))
                // {
                //     return ApiResponse<List<GetAssetResponseDTO>>
                //         .Fail("You do not have permission to view assets.");
                // }

                // ===============================
                // 3️⃣ FETCH FROM REPOSITORY
                // ===============================
                var assets =
                    await _unitOfWork.AssetRepository
                        .GetAssetsByFilterAsync(request.DTO);

                if (assets == null || assets.Count == 0)
                {
                    _logger.LogInformation(
                        "No Assets found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return new ApiResponse<List<GetAssetResponseDTO>>
                    {
                        IsSucceeded = true,
                        Message = "No assets found.",
                        Data = new List<GetAssetResponseDTO>()
                    };
                }

                _logger.LogInformation(
                    "Successfully retrieved {Count} assets for TenantId: {TenantId}",
                    assets.Count,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 4️⃣ RETURN RESPONSE
                // ===============================
                return new ApiResponse<List<GetAssetResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Assets fetched successfully.",
                    Data = assets
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching assets for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                return new ApiResponse<List<GetAssetResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching assets.",
                    Data = new List<GetAssetResponseDTO>()
                };
            }
        }
    }
}
