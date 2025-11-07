using AutoMapper;
 
using axionpro.application.Features.TicketFeatures.Classification.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.Classification.Handlers
{
    public class DeleteClassificationCommandHandler
        : IRequestHandler<DeleteClassificationCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteClassificationCommandHandler> _logger;

        public DeleteClassificationCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<DeleteClassificationCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteClassificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 Step 1: Validate input
                if (request == null || request.DTO == null || request.DTO.Id <= 0)
                {
                    _logger.LogWarning("⚠️ DeleteClassificationCommand received with invalid Classification Id.");
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid classification Id.",
                        Data = false
                    };
                }

                _logger.LogInformation("🗑️ Attempting to delete classification with Id: {Id}", request.DTO.Id);

                // 🧩 Step 2: Call repository to delete classification
                var isDeleted = await _unitOfWork.TicketClassificationRepository.DeleteAsync(request.DTO);

                // 🧩 Step 3: Check deletion status
                if (!isDeleted)
                {
                    _logger.LogWarning("⚠️ Classification with Id {Id} could not be deleted or not found.", request.DTO.Id);
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Classification not found or could not be deleted.",
                        Data = false
                    };
                }

                _logger.LogInformation("✅ Classification deleted successfully with Id: {Id}", request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Ticket classification deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting classification with Id: {Id}", request.DTO?.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while deleting the ticket classification.",
                    Data = false
                };
            }
        }
    }
}
