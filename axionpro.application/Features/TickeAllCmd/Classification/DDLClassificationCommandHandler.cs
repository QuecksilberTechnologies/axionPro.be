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
    public class DDLClassificationCommand: IRequest<ApiResponse<List<GetClassificationResponseDTO>>>
    {
        public DDLClassificationRequestDTO DTO { get; }

        public DDLClassificationCommand(DDLClassificationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ===============================
    // HANDLER (MASTER PATTERN)
    // ===============================
    public class DDLClassificationCommandHandler
     : IRequestHandler<DDLClassificationCommand, ApiResponse<List<GetClassificationResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DDLClassificationCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public DDLClassificationCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DDLClassificationCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetClassificationResponseDTO>>> Handle(
            DDLClassificationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
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

                // ===============================
                // 3️⃣ REPOSITORY CALL
                // ===============================
                var result = await _unitOfWork.TicketClassificationRepository
                    .GetDDLAsync(request.DTO.IsActive, validation.TenantId);

                // ===============================
                // 4️⃣ RESPONSE
                // ===============================
                return ApiResponse<List<GetClassificationResponseDTO>>
                    .Success(result, "Classification dropdown fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching classification DDL");
                throw;
            }
        }
    }

}