using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.Features.TicketFeatures.TicketHeader.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.TicketHeader.Handlers
{
    /// <summary>
    /// Handles the update of an existing Ticket Header.
    /// </summary>
    public class UpdateHeaderCommandHandler : IRequestHandler<UpdateHeaderCommand, ApiResponse<GetHeaderResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateHeaderCommandHandler> _logger;

        public UpdateHeaderCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<UpdateHeaderCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetHeaderResponseDTO>> Handle(UpdateHeaderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Validate request
                if (request == null || request.DTO == null || request.DTO.Id <= 0)
                {
                    _logger.LogWarning("⚠️ UpdateHeaderCommand received with invalid DTO or Id.");
                    return new ApiResponse<GetHeaderResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request or missing header data.",
                        Data = null
                    };
                }

                _logger.LogInformation("✏️ Attempting to update header with Id: {Id}", request.DTO.Id);

                // Step 2: Begin transaction
                await _unitOfWork.BeginTransactionAsync();

                // Step 3: Call repository to update header
                var updatedHeader = await _unitOfWork.TicketHeaderRepository.UpdateAsync(request.DTO);

                if (updatedHeader == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning("⚠️ Header with Id {Id} not found or could not be updated.", request.DTO.Id);
                    return new ApiResponse<GetHeaderResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Header not found or could not be updated.",
                        Data = null
                    };
                }

                // Step 4: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Header updated successfully. Id: {Id}", request.DTO.Id);

                return new ApiResponse<GetHeaderResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Ticket header updated successfully.",
                    Data = updatedHeader
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "❌ Error occurred while updating header with Id: {Id}", request.DTO?.Id);
                return new ApiResponse<GetHeaderResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while updating the ticket header.",
                    Data = null
                };
            }
        }
    }
}
