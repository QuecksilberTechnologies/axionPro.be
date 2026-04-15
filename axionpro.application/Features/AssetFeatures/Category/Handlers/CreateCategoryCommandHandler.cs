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
       : IRequest<ApiResponse<GetCategoryResponseDTO>>
    {
        public AddCategoryReqestDTO DTO { get; set; }

        public AddCategoryCommand(AddCategoryReqestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateCategoryCommandHandler
        : IRequestHandler<AddCategoryCommand, ApiResponse<GetCategoryResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public CreateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateCategoryCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<GetCategoryResponseDTO>> Handle(
            AddCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating Asset Category");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." });

                request.DTO.Prop ??= new();

                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;

                // ===============================
                // 2️⃣ INPUT VALIDATION
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.CategoryName))
                    throw new ValidationErrorException(
                        "Category name is required.",
                        new List<string> { "CategoryName cannot be empty." });

                // ===============================
                // 3️⃣ RBAC (OPTIONAL)
                // ===============================
                /*
                var hasPermission = await _permissionService.HasAccessAsync(
                    validation.RoleId,
                    Modules.AssetCategory,
                    Operations.Add);

                if (!hasPermission)
                    throw new UnauthorizedAccessException("No permission.");
                */

                // ===============================
                // 4️⃣ CALL REPOSITORY
                // ===============================
                var category = await _unitOfWork.AssetCategoryRepository
                    .AddAsync(request.DTO);

                // ===============================
                // 5️⃣ NULL SAFETY
                // ===============================
                if (category == null)
                {
                    _logger.LogWarning(
                        "Asset Category creation returned null for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    throw new Exception("Category creation failed.");
                }

                // ===============================
                // 6️⃣ SUCCESS
                // ===============================
                _logger.LogInformation(
                    "Asset Category created successfully for TenantId: {TenantId}",
                    request.DTO.Prop.TenantId);

                return ApiResponse<GetCategoryResponseDTO>
                    .Success(category, "Asset category created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while creating Asset Category for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }

}
