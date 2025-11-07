using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Features.TicketFeatures.Classification.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.TicketFeatures.Classification.Handlers
{
    public class GetAllClassificationCommandHandler : IRequestHandler<GetAllClassificationCommand, ApiResponse<List<GetClassificationResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllClassificationCommandHandler> _logger;

        public GetAllClassificationCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllClassificationCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetClassificationResponseDTO>>> Handle(GetAllClassificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("GetAllClassificationCommand request is null.");
                    return new ApiResponse<List<GetClassificationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. Unable to fetch ticket classifications.",
                        Data = null
                    };
                }

                _logger.LogInformation("Fetching all ticket classifications from database...");

                var ticketClassifications = await _unitOfWork.TicketClassificationRepository.GetAllAsync(request.DTO);

                if (ticketClassifications == null || !ticketClassifications.Any())
                {
                    _logger.LogWarning("No ticket classifications found in the system.");
                    return new ApiResponse<List<GetClassificationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No ticket classifications found.",
                        Data = null
                    };
                }

                var mappedResult = _mapper.Map<List<GetClassificationResponseDTO>>(ticketClassifications);

                _logger.LogInformation("Successfully fetched {Count} ticket classifications.", mappedResult.Count);

                return new ApiResponse<List<GetClassificationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Ticket classifications fetched successfully.",
                    Data = mappedResult
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ticket classifications.");
                return new ApiResponse<List<GetClassificationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while retrieving ticket classifications.",
                    Data = null
                };
            }
        }
    }
}
