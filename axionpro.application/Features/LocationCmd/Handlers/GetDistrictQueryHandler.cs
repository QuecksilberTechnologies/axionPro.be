using AutoMapper;
using axionpro.application.DTOS.Location;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
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

        public async Task<ApiResponse<List<GetDistrictOptionResponseDTO>>> Handle(GetDistrictQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate DTO
                if (request.DTO == null)
                {
                    _logger.LogWarning("Request DTO is null.");
                    return new ApiResponse<List<GetDistrictOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = new List<GetDistrictOptionResponseDTO>()
                    };
                }

                // ✅ Step 2: Validate TodaysDate
                if (!request.DTO.TodaysDate.HasValue)
                {
                    _logger.LogWarning("Today's date not provided in request.");
                    return new ApiResponse<List<GetDistrictOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Today's date is required.",
                        Data = new List<GetDistrictOptionResponseDTO>()
                    };
                }

                // ✅ Step 3: Validate IsActive flag
                if (request.DTO.IsActive != true)
                {
                    _logger.LogWarning("Inactive request received.");
                    return new ApiResponse<List<GetDistrictOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Inactive request. Cannot fetch districts.",
                        Data = new List<GetDistrictOptionResponseDTO>()
                    };
                }

                // ✅ Step 4: Validate StateId
                if (request.DTO.StateId <= 0)
                {
                    _logger.LogWarning("StateId is missing or invalid.");
                    return new ApiResponse<List<GetDistrictOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "StateId is required to fetch districts.",
                        Data = new List<GetDistrictOptionResponseDTO>()
                    };
                }

                // ✅ Step 5: Fetch Data
                var apiResponse = await _unitOfWork.LocationRepository.GetDistrictOptionAsync(request.DTO);

                // ✅ Step 6: Validate Response
                if (apiResponse?.Data == null || apiResponse.Data.Count == 0)
                {
                    _logger.LogWarning("No districts found for StateId {StateId}.", request.DTO.StateId);
                    return new ApiResponse<List<GetDistrictOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No districts found for the given state.",
                        Data = new List<GetDistrictOptionResponseDTO>()
                    };
                }

                // ✅ Step 7: Success Response
                _logger.LogInformation("Successfully retrieved {Count} districts for StateId {StateId}.",
                    apiResponse.Data.Count, request.DTO.StateId);

                return new ApiResponse<List<GetDistrictOptionResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Districts fetched successfully.",
                    Data = apiResponse.Data
                };
            }
            catch (Exception ex)
            {
                // ✅ Step 8: Exception Handling
                _logger.LogError(ex, "Error while fetching district list.");
                return new ApiResponse<List<GetDistrictOptionResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching districts.",
                    Data = new List<GetDistrictOptionResponseDTO>()
                };
            }
        }
    }
}
