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
     : IRequest<ApiResponse<List<GetStatusResponseDTO>>>
    {
        public GetStatusRequestDTO DTO { get; set; }

        public GetAllAssetStatusCommand(GetStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetAllStatusCommandHandler
        : IRequestHandler<GetAllAssetStatusCommand, ApiResponse<List<GetStatusResponseDTO>>>
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

        public async Task<ApiResponse<List<GetStatusResponseDTO>>> Handle(
      GetAllAssetStatusCommand request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Statuses");

                // ✅ VALIDATION
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." });

                request.DTO.Prop ??= new();
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ✅ FETCH DATA
                var pagedResult = await _unitOfWork.AssetStatusRepository
                    .GetAllAsync(request.DTO);

                // ✅ EMPTY CASE
                if (pagedResult == null || pagedResult.Data == null || !pagedResult.Data.Any())
                {
                    _logger.LogWarning(
                        "No Asset Status found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return ApiResponse<List<GetStatusResponseDTO>>.SuccessPaginatedPercentage(
                        Data: new List<GetStatusResponseDTO>(),
                        PageNumber: request.DTO.PageNumber <= 0 ? 1 : request.DTO.PageNumber,
                        PageSize: request.DTO.PageSize <= 0 ? 10 : request.DTO.PageSize,
                        TotalRecords: 0,
                        TotalPages: 0,
                        Message: "No Asset Status found.",
                        HasUploadedAll: null,
                        CompletionPercentage: null
                    );
                }

                _logger.LogInformation(
                    "Successfully retrieved {Count} records for TenantId: {TenantId}",
                    pagedResult.Data.Count,
                    request.DTO.Prop.TenantId);

                // ✅ FINAL RESPONSE (IMPORTANT 🔥)
                return ApiResponse<List<GetStatusResponseDTO>>.SuccessPaginatedPercentage(
                    Data: pagedResult.Data,
                    PageNumber: pagedResult.PageNumber,
                    PageSize: pagedResult.PageSize,
                    TotalRecords: pagedResult.TotalCount,
                    TotalPages: pagedResult.TotalPages,
                    Message: "Asset Status fetched successfully.",
                    HasUploadedAll: pagedResult.HasUploadedAll,
                    CompletionPercentage: pagedResult.CompletionPercentage
                );
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
