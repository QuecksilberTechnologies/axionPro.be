using AutoMapper;
using axionpro.application.DTOS.AssetDTO.type;
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
                //    "AssetType",   // 🔹 Module
                //    "View"         // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to view asset types.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var typeEntities = await _unitOfWork.AssetTypeRepository
                    .GetAllAsync(request.DTO);

                // ===============================
                // 5️⃣ HANDLE EMPTY DATA (IMPORTANT)
                // ===============================
                if (typeEntities == null || typeEntities.Count == 0)
                {
                    _logger.LogWarning(
                        "No Asset Types found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    // ✅ Empty list = success
                    return ApiResponse<List<GetTypeResponseDTO>>
                        .Success(new List<GetTypeResponseDTO>(), "No Asset Types found.");
                }

                // ===============================
                // 6️⃣ MAP ENTITY → DTO
                // ===============================
                var responseDTOs = _mapper.Map<List<GetTypeResponseDTO>>(typeEntities);

                _logger.LogInformation(
                    "Successfully retrieved {Count} Asset Types for TenantId: {TenantId}",
                    responseDTOs.Count,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 7️⃣ SUCCESS RESPONSE
                // ===============================
                return ApiResponse<List<GetTypeResponseDTO>>
                    .Success(responseDTOs, "Asset Types fetched successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Error occurred while fetching Asset Types for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }
}
