using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Features.AssetFeatures.Category.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Status.Handlers
{
   
      public class AddStatusCommand : IRequest<ApiResponse<GetStatusResponseDTO>>
    {
        public CreateStatusRequestDTO DTO { get; set; }

        public AddStatusCommand(CreateStatusRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
    public class AddStatusCommandHandler
     : IRequestHandler<AddStatusCommand, ApiResponse<GetStatusResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddStatusCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public AddStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AddStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<GetStatusResponseDTO>> Handle(
            AddStatusCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating Asset Status");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<GetStatusResponseDTO>
                        .Fail(validation.ErrorMessage);

                if (request?.DTO == null)
                    return ApiResponse<GetStatusResponseDTO>
                        .Fail("Invalid request data.");

                // 🔹 Inject TenantId from token/session
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ BUSINESS VALIDATION
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.StatusName))
                    return ApiResponse<GetStatusResponseDTO>
                        .Fail("StatusName is required.");

                if (string.IsNullOrWhiteSpace(request.DTO.ColorKey))
                    return ApiResponse<GetStatusResponseDTO>
                        .Fail("ColorKey is required.");

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
                var entity = _mapper.Map<AssetStatus>(request.DTO);

                entity.TenantId = validation.TenantId;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.AddedById = validation.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;

                // ===============================
                // 5️⃣ SAVE
                // ===============================
                await _unitOfWork.AssetStatusRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation(
                    "✅ Asset Status created successfully. Id: {Id}, TenantId: {TenantId}",
                    entity.Id, validation.TenantId
                );

                // ===============================
                // 6️⃣ MAP ENTITY → RESPONSE DTO
                // ===============================
                var response =
                    _mapper.Map<GetStatusResponseDTO>(entity);

                return ApiResponse<GetStatusResponseDTO>.Success(
                    response,
                    "Asset Status created successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while creating Asset Status. TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId
                );

                return ApiResponse<GetStatusResponseDTO>
                    .Fail("Something went wrong while adding asset status.");
            }
        }
    }

}