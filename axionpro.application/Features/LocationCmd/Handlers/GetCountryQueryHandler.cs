using AutoMapper;
using axionpro.application.DTOS.Location;
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

namespace axionpro.application.Features.LocationCmd.Handlers
{
    public class GetCountryQuery : IRequest<ApiResponse<List<GetCountryOptionResponseDTO>>>
    {
        public GetCountryOptionRequestDTO DTO { get; set; }

        public GetCountryQuery(GetCountryOptionRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetCountryQueryHandler : IRequestHandler<GetCountryQuery, ApiResponse<List<GetCountryOptionResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCountryQueryHandler> _logger;

        public GetCountryQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetCountryQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ApiResponse<List<GetCountryOptionResponseDTO>>> Handle(GetCountryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Input validation
                if (!request.DTO.TodaysDate.HasValue)
                {
                    _logger.LogWarning("Today's date not provided in request.");
                    return new ApiResponse<List<GetCountryOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Today's date is required.",
                        Data = new List<GetCountryOptionResponseDTO>()
                    };
                }

                if (request.DTO.IsActive != true)
                {
                    _logger.LogWarning("Request skipped because IsActive is false or null.");
                    return new ApiResponse<List<GetCountryOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Inactive request. Cannot fetch countries.",
                        Data = new List<GetCountryOptionResponseDTO>()
                    };
                }

                // ✅ Step 2: Log date
                var date = request.DTO.TodaysDate.Value.Date;
                _logger.LogInformation("Fetching countries for date: {Date}", date);

                // ✅ Step 3: Fetch data from repository
                var getCountry = await _unitOfWork.LocationRepository.GetCountryOptionAsync(request.DTO);

                // ✅ Step 4: Mapping
             

                if (getCountry == null || getCountry.Data.Count == 0)
                {
                    _logger.LogWarning("No countries found.");
                    return new ApiResponse<List<GetCountryOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No countries found.",
                        Data = new List<GetCountryOptionResponseDTO>()
                    };
                }

                // ✅ Step 5: Success response
                _logger.LogInformation("Successfully retrieved {Count} countries.", getCountry.Data.Count);
                return new ApiResponse<List<GetCountryOptionResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Countries fetched successfully.",
                    Data = getCountry.Data
                };
            }
            catch (Exception ex)
            {
                // ✅ Step 6: Error handling
                _logger.LogError(ex, "Error while fetching countries.");
                return new ApiResponse<List<GetCountryOptionResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching countries.",
                    Data = new List<GetCountryOptionResponseDTO>()
                };
            }
        }


    }
}
