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

    public class DeleteHeaderCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteHeaderRequestDTO DTO { get; set; }

        public DeleteHeaderCommand(DeleteHeaderRequestDTO dto)
        {
            DTO = dto;
        }

    }


    /// <summary>
    /// Handles the deletion of a Ticket Header (soft delete).
    /// </summary>
    public class DeleteHeaderCommandHandler
        : IRequestHandler<DeleteHeaderCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteHeaderCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        public DeleteHeaderCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<DeleteHeaderCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
     DeleteHeaderCommand request,
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
                //    OperationEnum.Delete);

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid request data.");
                
              

                // ===============================
                // 4️⃣ TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                var result = await _unitOfWork.TicketHeaderRepository
                    .DeleteAsync(request.DTO, validation.LoggedInEmployeeId);

                if (!result)
                    throw new ApiException("Header not found or could not be deleted.", 404);

                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 5️⃣ RESPONSE
                // ===============================
                return ApiResponse<bool>
                    .Success(true, "Ticket header deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting TicketHeader");
                throw;
            }
        }
    }
}
