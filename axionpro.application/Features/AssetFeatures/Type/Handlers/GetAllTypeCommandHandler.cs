using AutoMapper;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
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

                // ✅ VALIDATION
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." }
                    );

                request.DTO.Prop ??= new();
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ✅ FETCH PAGED DATA
                var pagedResult = await _unitOfWork.AssetTypeRepository
                    .GetAllAsync(request.DTO);

                // ✅ EMPTY CHECK
                if (pagedResult == null || pagedResult.Data == null || !pagedResult.Data.Any())
                {
                    _logger.LogWarning(
                        "No Asset Types found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return ApiResponse<List<GetTypeResponseDTO>>.SuccessPaginatedPercentage(
                        Data: new List<GetTypeResponseDTO>(),
                        PageNumber: request.DTO.PageNumber <= 0 ? 1 : request.DTO.PageNumber,
                        PageSize: request.DTO.PageSize <= 0 ? 10 : request.DTO.PageSize,
                        TotalRecords: 0,
                        TotalPages: 0,
                        Message: "No Asset Types found.",
                        HasUploadedAll: null,
                        CompletionPercentage: null
                    );
                }

                // ✅ NO MAPPING NEEDED (already DTO from repo 🔥)
                var responseDTOs = pagedResult.Data;

                _logger.LogInformation(
                    "Successfully retrieved {Count} Asset Types for TenantId: {TenantId}",
                    responseDTOs.Count,
                    request.DTO.Prop.TenantId);

                // ✅ FINAL RESPONSE (YOUR PATTERN)
                return ApiResponse<List<GetTypeResponseDTO>>.SuccessPaginatedPercentage(
                    Data: responseDTOs,
                    PageNumber: pagedResult.PageNumber,
                    PageSize: pagedResult.PageSize,
                    TotalRecords: pagedResult.TotalCount,
                    TotalPages: pagedResult.TotalPages,
                    Message: "Asset Types fetched successfully.",
                    HasUploadedAll: pagedResult.HasUploadedAll,
                    CompletionPercentage: pagedResult.CompletionPercentage
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching Asset Types for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }
}
