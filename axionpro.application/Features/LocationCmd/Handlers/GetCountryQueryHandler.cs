using AutoMapper;
using axionpro.application.DTOS.Location;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using axionpro.domain.Entity; 
using MediatR;
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
        public async Task<ApiResponse<List<GetCountryOptionResponseDTO>>> Handle(
       GetCountryQuery request,
       CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetCountry started (Open API)");

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

                var date = request.DTO.TodaysDate.Value.Date;

                _logger.LogInformation("Fetching countries for date: {Date}", date);

                // ===============================
                // 3️⃣ FETCH DATA
                // ===============================
                var result = await _unitOfWork
                    .LocationRepository
                    .GetCountryOptionAsync(request.DTO);

                var data = result?.Data ?? new List<GetCountryOptionResponseDTO>();

                _logger.LogInformation("✅ Retrieved {Count} countries", data.Count);

                // ===============================
                // 4️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetCountryOptionResponseDTO>>
                    .Success(data, "Countries fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetCountry");

                throw; // ✅ CRITICAL
            }
        }
    }
}
