using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
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

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class GetAllAssetCommand : IRequest<ApiResponse<List<GetAssetResponseDTO>>>
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetAllAssetCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllAssetCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetAllAssetCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService,
            ICommonRequestService commonRequestService)
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
            _fileStorageService = fileStorageService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetAssetResponseDTO>>> Handle(
            GetAllAssetCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Assets");

                // 1️⃣ COMMON VALIDATION
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                {
                    return ApiResponse<List<GetAssetResponseDTO>>.Fail(validation.ErrorMessage);
                }

                // 2️⃣ ASSIGN COMMON VALUES
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // 3️⃣ PERMISSION CHECK (optional)
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("ViewAsset"))
                // {
                //     return ApiResponse<List<GetAssetResponseDTO>>.Fail("You do not have permission to view assets.");
                // }

                // 4️⃣ FETCH DATA
                var assets = await _unitOfWork.AssetRepository.GetAssetsByFilterAsync(request.DTO);

                // ✅ Empty record bhi success maana jayega
                if (assets == null || assets.Count == 0)
                {
                    _logger.LogInformation(
                        "No assets found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return ApiResponse<List<GetAssetResponseDTO>>.Success(
                        new List<GetAssetResponseDTO>(),
                        "No assets found.");
                }

                // 5️⃣ PROJECTION / ENCRYPTION
                var encryptedList = ProjectionHelper.ToGetAssetResponseDTOs(
                    assets,
                    _idEncoderService,
                    validation.Claims.TenantEncriptionKey,
                    _config);

                _logger.LogInformation(
                    "Successfully retrieved {Count} assets for TenantId: {TenantId}",
                    encryptedList.Count,
                    request.DTO.Prop.TenantId);

                // 6️⃣ SUCCESS RESPONSE
                return ApiResponse<List<GetAssetResponseDTO>>.Success(
                    encryptedList,
                    "Assets fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching assets for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                return ApiResponse<List<GetAssetResponseDTO>>.Fail(
                    "An unexpected error occurred while fetching assets.",
                    new List<string> { ex.Message });
            }
        }
    }
}