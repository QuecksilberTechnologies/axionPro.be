using AutoMapper;
 
using axionpro.application.Features.TicketFeatures.TicketType.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TicketFeatures.TicketType.Handlers
{
    public class UpdateTicketTypeCommandHandler : IRequestHandler<UpdateTicketTypeCommand, ApiResponse<bool>>
    {
        private readonly ITicketTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<UpdateTicketTypeCommandHandler> _logger;

        public UpdateTicketTypeCommandHandler(
            ITicketTypeRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<UpdateTicketTypeCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<bool>> Handle(UpdateTicketTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate Request
                if (request.DTO == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. TicketType data is required.",
                        Data = false
                    };
                }

                if (request.DTO.Id <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "TicketType Id must be valid.",
                        Data = false
                    };
                }

                // 2️⃣ Update TicketType using Repository
                bool isUpdated = await _repository.UpdateAsync(request.DTO);

                if (!isUpdated)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "TicketType update failed. Either not found or no changes detected.",
                        Data = false
                    };
                }

                // 3️⃣ Commit Transaction (if using UnitOfWork)
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("TicketType updated successfully with Id {Id}", request.DTO.Id);

                // 4️⃣ Return Success Response
                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "TicketType updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating TicketType with Id {Id}", request.DTO?.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while updating TicketType: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
