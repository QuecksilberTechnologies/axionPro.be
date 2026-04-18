using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TickeAllCmd.TicketHeader.Handlers
{
    public class UpdateHeaderCommand : IRequest<ApiResponse<GetHeaderResponseDTO>>
    {
        public UpdateHeaderRequestDTO DTO { get; set; }

        public UpdateHeaderCommand(UpdateHeaderRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateHeaderCommandHandler
        : IRequestHandler<UpdateHeaderCommand, ApiResponse<GetHeaderResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateHeaderCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public UpdateHeaderCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateHeaderCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetHeaderResponseDTO>> Handle(
            UpdateHeaderCommand request,
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
                // 2️⃣ RBAC
                // ===============================
                //await _commonRequestService.HasAccessAsync(
                //    ModuleEnum.Ticket,
                //    OperationEnum.Update);

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid request data.");

                var dto = request.DTO;

                dto.Prop ??= new ExtraPropRequestDTO();
                dto.Prop.TenantId = validation.TenantId;

                if (string.IsNullOrWhiteSpace(dto.HeaderName))
                    throw new ValidationErrorException("Header name is required.");

                // ===============================
                // 4️⃣ TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                var result = await _unitOfWork.TicketHeaderRepository
                    .UpdateAsync(dto);

                if (result == null)
                    throw new ApiException("Header not found or could not be updated.", 404);

                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 5️⃣ RESPONSE
                // ===============================
                return ApiResponse<GetHeaderResponseDTO>
                    .Success(result, "Ticket header updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating TicketHeader");
                throw;
            }
        }
    }
}