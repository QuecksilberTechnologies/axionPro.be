using AutoMapper;
using axionpro.application.DTOS.AssetDTO.status;
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

namespace axionpro.application.Features.AssetFeatures.Status.Handlers
{
    public class GetAllAssetStatusCommand
        : IRequest<ApiResponse<List<GetStatusResponseDTO>>>
    {
        public GetStatusRequestDTO DTO { get; set; }

        public GetAllAssetStatusCommand(GetStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles fetching all Asset Statuses for a given tenant.
    /// </summary>
    public class GetAllStatusCommandHandler
        : IRequestHandler<GetAllAssetStatusCommand, ApiResponse<List<GetStatusResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllStatusCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public GetAllStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<List<GetStatusResponseDTO>>> Handle(
            GetAllAssetStatusCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Statuses");

                // ===============================
                // 1️⃣ COMMON VALIDATION (MANDATORY)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetStatusResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ PERMISSION CHECK (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("ViewAssetStatus"))
                // {
                //     return ApiResponse<List<GetStatusResponseDTO>>
                //         .Fail("You do not have permission to view asset status.");
                // }

                // ===============================
                // 3️⃣ FETCH DATA FROM REPOSITORY
                // ===============================
                var statusEntities =
                    await _unitOfWork.AssetStatusRepository
                        .GetAllAsync(request.DTO);

                if (statusEntities == null || statusEntities.Count == 0)
                {
                    _logger.LogWarning(
                        "No Asset Status found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return new ApiResponse<List<GetStatusResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Asset Status found.",
                        Data = new List<GetStatusResponseDTO>()
                    };
                }

                // ===============================
                // 4️⃣ MAP ENTITY → DTO
                // ===============================
                var responseDTOs =
                    _mapper.Map<List<GetStatusResponseDTO>>(statusEntities);

                _logger.LogInformation(
                    "Successfully retrieved {Count} Asset Status records for TenantId: {TenantId}",
                    responseDTOs.Count,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 5️⃣ RETURN RESPONSE
                // ===============================
                return new ApiResponse<List<GetStatusResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Asset Status fetched successfully.",
                    Data = responseDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching Asset Status for TenantId: {TenantId}",
                    request.DTO?.Prop?.TenantId);

                return new ApiResponse<List<GetStatusResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching asset status.",
                    Data = null
                };
            }
        }
    }
}
