using AutoMapper;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Features.TicketFeatures.TicketType.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TicketFeatures.TicketType.Handlers
{
    public class GetAllTicketTypeQueryHandler : IRequestHandler<GetAllTicketTypeQuery, ApiResponse<List<GetTicketTypeResponseDTO>>>
    {
        private readonly ITicketTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<GetAllTicketTypeQueryHandler> _logger;

        public GetAllTicketTypeQueryHandler(
            ITicketTypeRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<GetAllTicketTypeQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<List<GetTicketTypeResponseDTO>>> Handle(GetAllTicketTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate Request
                if (request == null)
                {
                    _logger.LogWarning("GetAllTicketTypeQuery request is null.");
                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request received.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }

                // 2️⃣ Fetch All Active Ticket Types
                var ticketTypes = await _repository.AllAsync(request.DTO);

                // 3️⃣ Validation: If no data found
                if (ticketTypes == null || !ticketTypes.Any())
                {
                    _logger.LogWarning("No TicketTypes found in database.");
                    return new ApiResponse<List<GetTicketTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No TicketTypes found.",
                        Data = new List<GetTicketTypeResponseDTO>()
                    };
                }

                _logger.LogInformation("Successfully retrieved {Count} TicketTypes.", ticketTypes.Count);

                // 4️⃣ Return Success Response
                return new ApiResponse<List<GetTicketTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "TicketTypes fetched successfully.",
                    Data = ticketTypes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all TicketTypes.");

                return new ApiResponse<List<GetTicketTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while fetching TicketTypes: {ex.Message}",
                    Data = new List<GetTicketTypeResponseDTO>()
                };
            }
        }
    }
}
