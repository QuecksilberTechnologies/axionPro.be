using AutoMapper;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR; 

using Microsoft.Extensions.Logging;

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
                        "Invalid request data.",
                        new List<string> { "Request DTO is required." }
                    );

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Inject values
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;

                // ===============================
                // 3️⃣ BUSINESS VALIDATION
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.StatusName))
                    throw new ValidationErrorException(
                        "StatusName is required.",
                        new List<string> { "StatusName cannot be empty." }
                    );

                if (string.IsNullOrWhiteSpace(request.DTO.ColorKey))
                    throw new ValidationErrorException(
                        "ColorKey is required.",
                        new List<string> { "ColorKey cannot be empty." }
                    );

                // ===============================
                // 4️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetStatus",   // 🔹 Module
                //    "Add"            // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to create asset status.");

                // ===============================
                // 5️⃣ MAP DTO → ENTITY
                // ===============================
                var entity = _mapper.Map<AssetStatus>(request.DTO);

                entity.TenantId = validation.TenantId;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.AddedById = validation.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;

                // ===============================
                // 6️⃣ SAVE (NO EXPLICIT TRANSACTION NEEDED)
                // ===============================
                await _unitOfWork.AssetStatusRepository.AddAsync(entity);
                await _unitOfWork.CommitAsync(); // ✔ required (save changes)

                _logger.LogInformation(
                    "Asset Status created successfully. Id: {Id}, TenantId: {TenantId}",
                    entity.Id,
                    validation.TenantId
                );

                // ===============================
                // 7️⃣ MAP ENTITY → RESPONSE DTO
                // ===============================
                var response = _mapper.Map<GetStatusResponseDTO>(entity);

                return ApiResponse<GetStatusResponseDTO>
                    .Success(response, "Asset Status created successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Error while creating Asset Status. TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId
                );

                throw;
            }
        }
    }

}