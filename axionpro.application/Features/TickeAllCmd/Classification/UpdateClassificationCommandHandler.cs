using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TickeAllCmd.Classification
{
    public class UpdateClassificationCommand : IRequest<ApiResponse<GetClassificationResponseDTO>>
    {
        public UpdateClassificationRequestDTO DTO { get; set; }

        public UpdateClassificationCommand(UpdateClassificationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateClassificationCommandHandler
        : IRequestHandler<UpdateClassificationCommand, ApiResponse<GetClassificationResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateClassificationCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public UpdateClassificationCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateClassificationCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetClassificationResponseDTO>> Handle(
            UpdateClassificationCommand request,
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

                if (string.IsNullOrWhiteSpace(dto.ClassificationName))
                    throw new ValidationErrorException("ClassificationName is required.");

                // ===============================
                // 4️⃣ TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                var result = await _unitOfWork.TicketClassificationRepository
                    .UpdateAsync(dto);

                if (result == null)
                    throw new ApiException("Classification not found or could not be updated.", 404);

                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 5️⃣ RESPONSE
                // ===============================
                return ApiResponse<GetClassificationResponseDTO>
                    .Success(result, "Ticket classification updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating TicketClassification");
                throw;
            }
        }
    }
}