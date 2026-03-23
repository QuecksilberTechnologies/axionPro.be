using AutoMapper;
using axionpro.application.DTOS.Location;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.LocationCmd.Handlers
{
    public class GetDistrictQuery : IRequest<ApiResponse<List<GetDistrictOptionResponseDTO>>>
    {
        public GetDistrictOptionRequestDTO? DTO { get; set; }

        public GetDistrictQuery(GetDistrictOptionRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetDistrictQueryHandler : IRequestHandler<GetDistrictQuery, ApiResponse<List<GetDistrictOptionResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDistrictQueryHandler> _logger;

        public GetDistrictQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetDistrictQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetDistrictOptionResponseDTO>>> Handle(
    GetDistrictQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetDistrict started (Open API)");

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

                if (request.DTO.StateId <= 0)
                    throw new ValidationErrorException("StateId is required to fetch districts.");

                // ===============================
                // 3️⃣ FETCH DATA
                // ===============================
                var result = await _unitOfWork
                    .LocationRepository
                    .GetDistrictOptionAsync(request.DTO);

                var data = result?.Data ?? new List<GetDistrictOptionResponseDTO>();

                _logger.LogInformation(
                    "✅ Retrieved {Count} districts for StateId {StateId}",
                    data.Count,
                    request.DTO.StateId);

                // ===============================
                // 4️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetDistrictOptionResponseDTO>>
                    .Success(data, "Districts fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetDistrict");

                throw; // ✅ CRITICAL
            }
        }
    }
}
