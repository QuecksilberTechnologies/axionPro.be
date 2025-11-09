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
        public async Task<ApiResponse<List<GetStateOptionResponseDTO>>> Handle(GetStateQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.DTO == null)
                {
                    _logger.LogWarning("Request DTO is null.");
                    return new ApiResponse<List<GetStateOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = new List<GetStateOptionResponseDTO>()
                    };
                }

                // ✅ Step 2: Validate TodaysDate
                if (!request.DTO.TodaysDate.HasValue)
                {
                    _logger.LogWarning("Today's date not provided in request.");
                    return new ApiResponse<List<GetStateOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Today's date is required.",
                        Data = new List<GetStateOptionResponseDTO>()
                    };
                }

                // ✅ Step 3: Validate IsActive flag
                //if (request.DTO.IsActive != true)
                //{
                //    _logger.LogWarning("Inactive request received.");
                //    return new ApiResponse<List<GetStateOptionResponseDTO>>
                //    {
                //        IsSucceeded = false,
                //        Message = "Inactive request. Cannot fetch districts.",
                //        Data = new List<GetStateOptionResponseDTO>()
                //    };
                //}

                // ✅ Step 4: Validate StateId
                if (request.DTO.CountryId <= 0)
                {
                    _logger.LogWarning("CountryId is missing or invalid.");
                    return new ApiResponse<List<GetStateOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "StateId is required to fetch districts.",
                        Data = new List<GetStateOptionResponseDTO>()
                    };
                }

                var getState = await _unitOfWork.LocationRepository.GetStateOptionAsync(request.DTO);

              

                _logger.LogInformation("Successfully retrieved {Count} Operations.", getState.Data.Count);
                return new ApiResponse<List<GetStateOptionResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Country fetched successfully.",
                    Data = getState.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Operations.");
                return new ApiResponse<List<GetStateOptionResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Country not fetched .",
                    Data = null
                };
            }
        }



    }
}
