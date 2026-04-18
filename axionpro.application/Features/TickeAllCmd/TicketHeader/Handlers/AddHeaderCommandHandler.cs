using AutoMapper;
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
    public class AddHeaderCommand : IRequest<ApiResponse<GetHeaderResponseDTO>>
    {
        public AddHeaderRequestDTO DTO { get; set; }

        public AddHeaderCommand(AddHeaderRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class AddHeaderCommandHandler
        : IRequestHandler<AddHeaderCommand, ApiResponse<GetHeaderResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddHeaderCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public AddHeaderCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<AddHeaderCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetHeaderResponseDTO>> Handle(
            AddHeaderCommand request,
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
                //    OperationEnum.Create);

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
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

                var result = await _unitOfWork.TicketHeaderRepository.AddAsync(dto);

                if (result == null)
                    throw new ApiException("Failed to add header.", 500);

                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 5️⃣ RESPONSE
                // ===============================
                return ApiResponse<GetHeaderResponseDTO>
                    .Success(result, "Header added successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error in AddHeaderCommandHandler");
                throw;
            }
        }
    }
}