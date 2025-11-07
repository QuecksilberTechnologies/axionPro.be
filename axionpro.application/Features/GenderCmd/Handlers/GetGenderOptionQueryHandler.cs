using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Gender;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.GenderCmd.Handlers
{
    // ✅ Query Definition
    public class GetGenderOptionQuery : IRequest<ApiResponse<List<GetGenderOptionResponseDTO>>>
    {
        public GetOptionRequestDTO OptionDTO { get; set; }

        public GetGenderOptionQuery(GetOptionRequestDTO optionDTO)
        {
            OptionDTO = optionDTO;
        }
    }

    // ✅ Query Handler
    public class GetGenderOptionQueryHandler : IRequestHandler<GetGenderOptionQuery, ApiResponse<List<GetGenderOptionResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetGenderOptionQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public GetGenderOptionQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetGenderOptionQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
        }

        public async Task<ApiResponse<List<GetGenderOptionResponseDTO>>> Handle(GetGenderOptionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate Input
                if (request?.OptionDTO == null)
                {
                    _logger.LogWarning("Invalid gender option request: OptionDTO is null.");
                    return new ApiResponse<List<GetGenderOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = new List<GetGenderOptionResponseDTO>()
                    };
                }

                // ✅ Step 2: Check TodaysDate
                if (request.OptionDTO.TodaysDate == null)
                {
                    _logger.LogWarning("TodaysDate is null in gender option request.");
                    return new ApiResponse<List<GetGenderOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Today's date is required to fetch gender options.",
                        Data = new List<GetGenderOptionResponseDTO>()
                    };
                }

                // ✅ Step 3: Call Repository
                var genders = await _unitOfWork.GenderRepository.GetOptionAsync(request.OptionDTO);
               
                // ✅ Step 4: Handle No Data
                if (genders?.Data == null || !genders.Data.Any())
                {
                    _logger.LogWarning("No gender options found for request: {@Request}", request.OptionDTO);
                    return new ApiResponse<List<GetGenderOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = genders?.Message ?? "No gender data found.",
                        Data = new List<GetGenderOptionResponseDTO>()
                    };
                }

                // ✅ Step 5: Logging & Response
                _logger.LogInformation("Successfully retrieved {Count} gender options.", genders.Data.Count);

                return new ApiResponse<List<GetGenderOptionResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = genders.Message ?? "Gender options fetched successfully.",
                    Data = genders.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching gender options for request: {@Request}", request?.OptionDTO);

                return new ApiResponse<List<GetGenderOptionResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to fetch gender options due to an internal error.",
                    Data = new List<GetGenderOptionResponseDTO>()
                };
            }
        }
    }
}
