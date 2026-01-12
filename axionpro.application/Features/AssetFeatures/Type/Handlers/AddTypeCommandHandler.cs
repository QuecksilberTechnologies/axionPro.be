using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Features.AssetFeatures.Status.Handlers;
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
    public class AddTypeCommand : IRequest<ApiResponse<List<GetTypeResponseDTO>>>
    {
        public AddTypeRequestDTO DTO { get; set; }

        public AddTypeCommand(AddTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class AddTypeCommandHandler : IRequestHandler<AddTypeCommand, ApiResponse<List<GetTypeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public AddTypeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AddTypeCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }
        public async Task<ApiResponse<List<GetTypeResponseDTO>>> Handle(AddTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Adding Asset Type: {TypeName}", request.DTO.TypeName);

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetTypeResponseDTO>>
                        .Fail(validation.ErrorMessage);

                if (request?.DTO == null)
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("Invalid request data.");

                // 🔹 Inject TenantId from token/session
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;

                // ===============================
                // 2️⃣ BUSINESS VALIDATION
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.TypeName))
                    return ApiResponse<List<GetTypeResponseDTO>>
                         .Fail("Type Name is required.");

               

                // ===============================
                // 3️⃣ PERMISSION CHECK (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("AddAssetStatus"))
                //     return ApiResponse<GetStatusResponseDTO>
                //         .Fail("You do not have permission to add asset status.");

                // ===============================
                // 4️⃣ MAP DTO → ENTITY
                // ===============================

                // Step 2️⃣: Add asset type via repository
                var addedList = await _unitOfWork.AssetTypeRepository.AddAsync(request.DTO);

                if (addedList == null || addedList.Count == 0)
                {
                    _logger.LogWarning("Asset type insertion failed for TypeName: {TypeName}", request.DTO.TypeName);
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("Failed to add asset type.");
                }

                // Step 3️⃣: Logging success
                _logger.LogInformation("Successfully added asset type: {TypeName}", request.DTO.TypeName);

                // Step 4️⃣: Return success response
                return ApiResponse<List<GetTypeResponseDTO>>.Success(
                    addedList,
                    "Asset Type added successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding asset type: {TypeName}", request.DTO?.TypeName);
                return ApiResponse<List<GetTypeResponseDTO>>.Fail("An unexpected error occurred while adding asset type.");
            }
        }
    }
}
