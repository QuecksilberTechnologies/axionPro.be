using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
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
      : IRequest<ApiResponse<PagedResponseDTO<GetClassificationResponseDTO>>>
    {
        public GetAllClassificationRequestDTO DTO { get; }

        public GetAllClassificationCommand(GetAllClassificationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ===============================
    // HANDLER (MASTER PATTERN)
    // ===============================
    public class GetAllClassificationCommandHandler
     : IRequestHandler<GetAllClassificationCommand, ApiResponse<PagedResponseDTO<GetClassificationResponseDTO>>>
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

        public async Task<ApiResponse<PagedResponseDTO<GetClassificationResponseDTO>>> Handle(
     GetAllClassificationCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetAllClassification started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new ExtraPropRequestDTO();
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ RBAC (OPTIONAL)
                // ===============================
                // await _commonRequestService.HasAccessAsync(
                //     ModuleEnum.Ticket,
                //     OperationEnum.View);

                // ===============================
                // 4️⃣ REPOSITORY CALL
                // ===============================
                var result = await _unitOfWork.TicketClassificationRepository
                    .GetAllAsync(request.DTO);

                if (result == null)
                    throw new ApiException("Failed to fetch classifications.", 500);

                _logger.LogInformation("GetAllClassification success");

                // ===============================
                // 5️⃣ FINAL RESPONSE
                // ===============================
                return ApiResponse<PagedResponseDTO<GetClassificationResponseDTO>>
                    .SuccessPaginatedOnly(
                        Data: result,
                        PageNumber: result.PageNumber,
                        PageSize: result.PageSize,
                        TotalRecords: result.TotalCount,
                        TotalPages: result.TotalPages,
                        Message: result.Data.Any()
                            ? "Classifications retrieved successfully."
                            : "No classifications found."
                      
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching classifications | UserId: {UserId}",
                    request.DTO?.Prop?.UserEmployeeId);

                throw;
            }
        }
    }


}