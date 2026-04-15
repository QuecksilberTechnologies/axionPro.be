using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
namespace axionpro.application.Features.AssetFeatures.Status.Handlers
{
    public class GetAllAssetStatusCommand
     : IRequest<ApiResponse<PagedResponseDTO<GetStatusResponseDTO>>>
    {
        public GetStatusRequestDTO DTO { get; set; }

        public GetAllAssetStatusCommand(GetStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetAllStatusCommandHandler
        : IRequestHandler<GetAllAssetStatusCommand, ApiResponse<PagedResponseDTO<GetStatusResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllStatusCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public GetAllStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<PagedResponseDTO<GetStatusResponseDTO>>> Handle(
            GetAllAssetStatusCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Statuses");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL CHECK
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." });

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ RBAC (OPTIONAL - UNCOMMENT IF NEEDED)
                // ===============================
                /*
                var hasPermission = await _permissionService.HasAccessAsync(
                    validation.RoleId,
                    Modules.AssetStatus,
                    Operations.View);

                if (!hasPermission)
                    throw new UnauthorizedAccessException("No permission.");
                */

                // ===============================
                // 4️⃣ FETCH DATA (ALREADY PAGED)
                // ===============================
                var result = await _unitOfWork.AssetStatusRepository
                    .GetAllAsync(request.DTO);

                // ===============================
                // 5️⃣ EMPTY HANDLING
                // ===============================
                if (result == null || result.Items.Count == 0)
                {
                    _logger.LogWarning(
                        "No Asset Status found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return ApiResponse<PagedResponseDTO<GetStatusResponseDTO>>
                        .Success(new PagedResponseDTO<GetStatusResponseDTO>
                        {
                            Items = new List<GetStatusResponseDTO>(),
                            TotalCount = 0,
                            PageNumber = request.DTO.PageNumber,
                            PageSize = request.DTO.PageSize
                        }, "No Asset Status found.");
                }

                _logger.LogInformation(
                    "Successfully retrieved {Count} records for TenantId: {TenantId}",
                    result.Items.Count,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 6️⃣ SUCCESS
                // ===============================
                return ApiResponse<PagedResponseDTO<GetStatusResponseDTO>>                  
                     .Success(new PagedResponseDTO<GetStatusResponseDTO>
                     {
                         Items = new List<GetStatusResponseDTO>(),
                         TotalCount = result.TotalCount,
                         PageNumber = request.DTO.PageNumber,
                         PageSize = request.DTO.PageSize
                     }, "No Asset Status found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching Asset Status for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }
}
