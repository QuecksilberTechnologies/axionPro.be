using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TickeAllCmd.Classification
{
    // ===============================
    // COMMAND
    // ===============================
    public class GetAllClassificationCommand
        : IRequest<ApiResponse<List<GetClassificationResponseDTO>>>
    {
        public GetClassificationRequestDTO DTO { get; }

        public GetAllClassificationCommand(GetClassificationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ===============================
    // HANDLER (MASTER PATTERN)
    // ===============================
    public class GetAllClassificationCommandHandler
        : IRequestHandler<GetAllClassificationCommand, ApiResponse<List<GetClassificationResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllClassificationCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public GetAllClassificationCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllClassificationCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<List<GetClassificationResponseDTO>>> Handle(
            GetAllClassificationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🚀 GetAllClassification started");

                // ===============================
                // 1️⃣ VALIDATION (MASTER PATTERN)
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION (OPTIONAL - SAME AS EMPLOYEE)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.TicketClassification,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to view classification.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var responseDTO =
                    await _unitOfWork.TicketClassificationRepository
                        .GetAllAsync(request.DTO);

                // ===============================
                // 5️⃣ EMPTY HANDLING (MASTER PATTERN)
                // ===============================
                var items = responseDTO?.Data ?? new List<GetClassificationResponseDTO>();

                var resultList = items.Any()
                    ? items
                    : new List<GetClassificationResponseDTO>();

                _logger.LogInformation("GetAllClassification success");

                // ===============================
                // 6️⃣ FINAL RESPONSE (MASTER PATTERN)
                // ===============================
                return ApiResponse<List<GetClassificationResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: resultList,
                        Message: items.Any()
                            ? "Classifications retrieved successfully."
                            : "No classifications found.",
                        PageNumber: responseDTO?.PageNumber ?? 1,
                        PageSize: responseDTO?.PageSize ?? 0,
                        TotalRecords: responseDTO?.TotalCount ?? 0,
                        TotalPages: responseDTO?.TotalPages ?? 0,
                        CompletionPercentage: responseDTO?.CompletionPercentage ?? 0,
                        HasUploadedAll: responseDTO?.HasUploadedAll ?? false
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error fetching classifications | UserId: {UserId}",
                    request.DTO?.Prop?.UserEmployeeId);

                throw; // 🔥 IMPORTANT
            }
        }
    }
}