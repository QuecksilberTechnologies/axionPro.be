using AutoMapper;
using axionpro.application.DTOS.AssetDTO.type;
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

namespace axionpro.application.Features.AssetFeatures.Type.Handlers
{
    public class GetAllTypeCommand
        : IRequest<ApiResponse<List<GetTypeResponseDTO>>>
    {
        public GetTypeRequestDTO DTO { get; set; }

        public GetAllTypeCommand(GetTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles fetching all Asset Types for a given tenant.
    /// </summary>
    public class GetAllTypeCommandHandler
        : IRequestHandler<GetAllTypeCommand, ApiResponse<List<GetTypeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public GetAllTypeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllTypeCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<List<GetTypeResponseDTO>>> Handle(
            GetAllTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Types");

                // ===============================
                // 1️⃣ COMMON VALIDATION (MANDATORY)
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetTypeResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ PERMISSION CHECK (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("ViewAssetType"))
                // {
                //     return ApiResponse<List<GetTypeResponseDTO>>
                //         .Fail("You do not have permission to view asset types.");
                // }

                // ===============================
                // 3️⃣ FETCH FROM REPOSITORY
                // ===============================
                var typeEntities =
                    await _unitOfWork.AssetTypeRepository
                        .GetAllAsync(request.DTO);

                if (typeEntities == null || typeEntities.Count == 0)
                {
                    _logger.LogWarning(
                        "No Asset Types found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return new ApiResponse<List<GetTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Asset Types found.",
                        Data = new List<GetTypeResponseDTO>()
                    };
                }

                // ===============================
                // 4️⃣ MAP ENTITY → DTO
                // ===============================
                var responseDTOs =
                    _mapper.Map<List<GetTypeResponseDTO>>(typeEntities);

                _logger.LogInformation(
                    "Successfully retrieved {Count} Asset Types for TenantId: {TenantId}",
                    responseDTOs.Count,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 5️⃣ RETURN RESPONSE
                // ===============================
                return new ApiResponse<List<GetTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Asset Types fetched successfully.",
                    Data = responseDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching Asset Types for TenantId: {TenantId}",
                    request.DTO?.Prop?.TenantId);

                return new ApiResponse<List<GetTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching asset types.",
                    Data = null
                };
            }
        }
    }
}
