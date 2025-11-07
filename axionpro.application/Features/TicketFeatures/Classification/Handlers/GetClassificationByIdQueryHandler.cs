using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Classification;
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
    public class GetClassificationByIdQueryHandler : IRequestHandler<GetClassificationByIdQuery, ApiResponse<GetClassificationResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetClassificationByIdQueryHandler> _logger;

        public GetClassificationByIdQueryHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetClassificationByIdQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<GetClassificationResponseDTO>> Handle(GetClassificationByIdQuery request, CancellationToken cancellationToken)
        {
            // 🧩 Step 1: Validate input
            if (request == null || request == null)
            {
                _logger.LogWarning("⚠️ GetClassificationByIdQuery received with null or invalid DTO.");
                return new ApiResponse<GetClassificationResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Invalid request. DTO cannot be null.",
                    Data = null
                };
            }

            try
            {
                _logger.LogInformation("🔍 Fetching classification by Id: {Id}", request.Dto.Id);

                // 🧩 Step 2: Fetch classification by Id from repository
                var classification = await _unitOfWork.TicketClassificationRepository.GetByIdAsync(request.Dto);

                // 🧩 Step 3: Check if classification exists
                if (classification == null)
                {
                    _logger.LogInformation("ℹ️ No classification found for Id: {Id}", request.Dto.Id);
                    return new ApiResponse<GetClassificationResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "No classification found for the provided Id.",
                        Data = null
                    };
                }

                // 🧩 Step 4: Map to response DTO
                var mappedResult = _mapper.Map<GetClassificationResponseDTO>(classification);

                // 🧩 Step 5: Success log and return
                _logger.LogInformation("✅ Successfully fetched classification. Id: {Id}", request.Dto.Id);

                return new ApiResponse<GetClassificationResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Ticket classification fetched successfully.",
                    Data = mappedResult
                };
            }
            catch (Exception ex)
            {
                // 🧩 Step 6: Exception handling
                _logger.LogError(ex, "❌ Error occurred while fetching classification info. Id: {Id}", request.Dto.Id);
                return new ApiResponse<GetClassificationResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching ticket classification details.",
                    Data = null
                };
            }
        }
    }
}
