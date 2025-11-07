using AutoMapper;
using axionpro.application.Features.TicketFeatures.TicketHeader.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Handlers
{
    /// <summary>
    /// Handles the deletion of a Ticket Header (soft delete).
    /// </summary>
    public class DeleteHeaderCommandHandler
        : IRequestHandler<DeleteHeaderCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteHeaderCommandHandler> _logger;

        public DeleteHeaderCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<DeleteHeaderCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteHeaderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input
                if (request == null || request.DTO == null || request.DTO.Id <= 0)
                {
                    _logger.LogWarning("⚠️ DeleteHeaderCommand received with invalid Header Id.");
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid header Id.",
                        Data = false
                    };
                }

                _logger.LogInformation("🗑️ Attempting to delete header with Id: {Id}", request.DTO.Id);

                // ✅ Step 2: Call repository to delete header
                var isDeleted = await _unitOfWork.TicketHeaderRepository.DeleteAsync(request.DTO);

                // ✅ Step 3: Check deletion status
                if (!isDeleted)
                {
                    _logger.LogWarning("⚠️ Header with Id {Id} could not be deleted or not found.", request.DTO.Id);
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Header not found or could not be deleted.",
                        Data = false
                    };
                }

                _logger.LogInformation("✅ Header deleted successfully with Id: {Id}", request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Ticket header deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting header with Id: {Id}", request.DTO?.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while deleting the ticket header.",
                    Data = false
                };
            }
        }
    }
}
