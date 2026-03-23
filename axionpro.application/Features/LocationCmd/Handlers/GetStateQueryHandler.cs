using AutoMapper;
using axionpro.application.DTOS.Location;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.LocationCmd.Handlers
{
    public class GetStateQuery : IRequest<ApiResponse<List<GetStateOptionResponseDTO>>>
    {
        public GetStateOptionRequestDTO? DTO { get; set; }

        public GetStateQuery(GetStateOptionRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetStateQueryHandler : IRequestHandler<GetStateQuery, ApiResponse<List<GetStateOptionResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetStateQueryHandler> _logger;

        public GetStateQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetStateQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ApiResponse<List<GetStateOptionResponseDTO>>> Handle(
    GetStateQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetState started (Open API)");

                // ===============================
                // 1️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                // ===============================
                // 2️⃣ INPUT VALIDATION
                // ===============================
                if (!request.DTO.TodaysDate.HasValue)
                    throw new ValidationErrorException("Today's date is required.");

                if (request.DTO.CountryId <= 0)
                    throw new ValidationErrorException("CountryId is required to fetch states.");

                // ===============================
                // 3️⃣ FETCH DATA
                // ===============================
                var result = await _unitOfWork
                    .LocationRepository
                    .GetStateOptionAsync(request.DTO);

                var data = result?.Data ?? new List<GetStateOptionResponseDTO>();

                _logger.LogInformation(
                    "✅ Retrieved {Count} states for CountryId {CountryId}",
                    data.Count,
                    request.DTO.CountryId);

                // ===============================
                // 4️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetStateOptionResponseDTO>>
                    .Success(data, "States fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetState");

                throw; // ✅ CRITICAL
            }
        }


    }
}
