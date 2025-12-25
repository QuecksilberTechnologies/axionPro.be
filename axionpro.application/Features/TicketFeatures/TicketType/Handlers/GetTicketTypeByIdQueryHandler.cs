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
    public class GetTicketTypeByIdQueryHandler : IRequestHandler<GetTicketTypeByIdQuery, ApiResponse<GetTicketTypeResponseDTO>>
    {
        private readonly ITicketTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<GetTicketTypeByIdQueryHandler> _logger;

        public GetTicketTypeByIdQueryHandler(
            ITicketTypeRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<GetTicketTypeByIdQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<GetTicketTypeResponseDTO>> Handle(GetTicketTypeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validation
                if (request == null || request.DTO.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid request received in GetTicketTypeByIdQuery. Id = {Id}", request.DTO?.Id);
                    return new ApiResponse<GetTicketTypeResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. TicketType Id must be greater than zero.",
                        Data = null
                    };
                }

                // 2️⃣ Fetch from repository
                var Entity = await _repository.GetByIdAsync(request.DTO.Id);
                if (Entity == null)
                {
                    _logger.LogWarning("⚠️ No TicketType found for Id = {Id}", request.DTO.Id);
                    return new ApiResponse<GetTicketTypeResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = $"No TicketType found for Id = {request.DTO.Id}.",
                        Data = null
                    };
                }
 

                // 4️⃣ Optional: Commit transaction if needed (not required for read ops, but safe)
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("✅ Successfully fetched TicketType (Id = {Id})", request.DTO.Id);

                // 5️⃣ Success response
                return new ApiResponse<GetTicketTypeResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "TicketType fetched successfully.",
                    Data = Entity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching TicketType by Id = {Id}", request.DTO?.Id);

                return new ApiResponse<GetTicketTypeResponseDTO>
                {
                    IsSucceeded = false,
                    Message = $"An unexpected error occurred while fetching TicketType: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
