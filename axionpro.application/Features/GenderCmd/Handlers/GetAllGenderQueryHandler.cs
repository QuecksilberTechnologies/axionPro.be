using AutoMapper;
using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.Leave;
using axionpro.application.Features.GenderCmd.Queries;
using axionpro.application.Features.LeaveCmd.Handlers;
using axionpro.application.Features.LeaveCmd.Queries;
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

namespace axionpro.application.Features.GenderCmd.Handlers
{
    public class GetAllGenderQueryHandler : IRequestHandler<GetAllGenderQuery, ApiResponse<List<GetGenderResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllGenderQueryHandler> _logger;

        public GetAllGenderQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllGenderQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetGenderResponseDTO>>> Handle(GetAllGenderQuery request, CancellationToken cancellationToken)
        {
            try
            {
               
                if (request.DTO == null)
                {
                    return new ApiResponse<List<GetGenderResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Gender list found.",
                        Data = new List<GetGenderResponseDTO>()
                    };
                }
                // 🔹 Repository se data fetch karo
                IEnumerable<Gender> genders = await _unitOfWork.GenderRepository.GetAllAsync();

                // 🔹 Validation: Agar list null ya empty hai
                if (genders == null || !genders.Any())
                {
                    _logger.LogWarning("⚠️ No Genders found in database.");

                    return new ApiResponse<List<GetGenderResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Genders found.",
                        Data = new List<GetGenderResponseDTO>()
                    };
                }

                // 🔹 Map entity -> DTO
                var GenderDTOs = _mapper.Map<List<GetGenderResponseDTO>>(genders);

                _logger.LogInformation("✅ Successfully retrieved {Count} Genders.", GenderDTOs.Count);

                return new ApiResponse<List<GetGenderResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Genders fetched successfully.",
                    Data = GenderDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching LeaveTypes.");

                return new ApiResponse<List<GetGenderResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while processing your request.",
                    Data = new List<GetGenderResponseDTO>()
                };
            }
        }


    }
}
     