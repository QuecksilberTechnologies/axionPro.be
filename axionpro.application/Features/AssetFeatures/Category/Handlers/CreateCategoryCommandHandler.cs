using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

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
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                // ❌ Old: return Fail
                // ✅ New: throw (middleware handle karega)
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

                // Assign Tenant
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;

                if (validation.TenantId <= 0)
                {
                    _logger.LogWarning(
                        "Invalid TenantId provided in AddAsync: {TenantId}",
                        validation.TenantId);

                    throw new ValidationErrorException(
                        "Invalid tenant.",
                        new List<string> { "TenantId must be valid." }
                    );
                }

                // ===============================
                // 3️⃣ INPUT VALIDATION
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.CategoryName))
                {
                    _logger.LogWarning(
                        "CategoryName is missing for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    throw new ValidationErrorException(
                        "Category name is required.",
                        new List<string> { "CategoryName cannot be empty." }
                    );
                }

                // ===============================
                // 4️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetCategory",   // 🔹 Module
                //    "Add"             // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to create asset category.");

                // ===============================
                // 5️⃣ CALL REPOSITORY (NO TRANSACTION NEEDED)
                // ===============================
                var categories = await _unitOfWork.AssetCategoryRepository
                    .AddAsync(request.DTO);

                // ===============================
                // 6️⃣ HANDLE NO DATA RETURN (EDGE CASE)
                // ===============================
                if (categories == null || categories.Count == 0)
                {
                    _logger.LogWarning(
                        "Asset Category created but no data returned for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return ApiResponse<List<GetCategoryResponseDTO>>
                        .Success(new List<GetCategoryResponseDTO>(),
                            "Asset category created, but no data returned.");
                }

                // ===============================
                // 7️⃣ SUCCESS RESPONSE
                // ===============================
                _logger.LogInformation(
                    "Asset Category created successfully for TenantId: {TenantId}",
                    request.DTO.Prop.TenantId);

                return ApiResponse<List<GetCategoryResponseDTO>>
                    .Success(categories, "Asset category created successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Error occurred while creating Asset Category for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }


}
