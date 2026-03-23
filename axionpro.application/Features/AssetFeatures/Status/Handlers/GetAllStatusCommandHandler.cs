using AutoMapper;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using axionpro.domain.Entity; 
using MediatR;
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
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                // ❌ Old: return Fail
                // ✅ New: throw
                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." }
                    );

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Assign values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetStatus",   // 🔹 Module
                //    "View"           // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to view asset status.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var statusEntities = await _unitOfWork.AssetStatusRepository
                    .GetAllAsync(request.DTO);

                // ===============================
                // 5️⃣ HANDLE EMPTY DATA (IMPORTANT)
                // ===============================
                if (statusEntities == null || statusEntities.Count == 0)
                {
                    _logger.LogWarning(
                        "No Asset Status found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    // ✅ Empty list = success
                    return ApiResponse<List<GetStatusResponseDTO>>
                        .Success(new List<GetStatusResponseDTO>(), "No Asset Status found.");
                }

                // ===============================
                // 6️⃣ MAP ENTITY → DTO
                // ===============================
                var responseDTOs = _mapper.Map<List<GetStatusResponseDTO>>(statusEntities);

                _logger.LogInformation(
                    "Successfully retrieved {Count} Asset Status records for TenantId: {TenantId}",
                    responseDTOs.Count,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 7️⃣ SUCCESS RESPONSE
                // ===============================
                return ApiResponse<List<GetStatusResponseDTO>>
                    .Success(responseDTOs, "Asset Status fetched successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Error occurred while fetching Asset Status for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }
}
