using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Features.AssetFeatures.Category.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    /// <summary>
    /// Handles fetching all Asset Categories for a given tenant.
    /// </summary>
    public class GetAllCategoryCommandHandler
        : IRequestHandler<GetAllCategoryCommand, ApiResponse<List<GetCategoryResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCategoryCommandHandler> _logger;

        public GetAllCategoryCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCategoryCommand request to retrieve all categories.
        /// </summary>
        public async Task<ApiResponse<List<GetCategoryResponseDTO>>> Handle(GetAllCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Categories ");

                // ✅ Validation Check
                if (request.DTO == null || request.DTO.TenantId <= 0)
                {
                    _logger.LogWarning("Invalid GetAllCategory request. TenantId is missing or invalid.");
                    return new ApiResponse<List<GetCategoryResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. TenantId is required.",
                        Data = null
                    };
                }

                // ✅ Fetch data from repository
                var categoryEntities = await _unitOfWork.AssetCategoryRepository.GetAllAsync(request.DTO);

                if (categoryEntities == null || categoryEntities.Count == 0)
                {
                    _logger.LogWarning("No Asset Categories found for TenantId: {TenantId}", request.DTO.TenantId);
                    return new ApiResponse<List<GetCategoryResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No Asset Categories found.",
                        Data = new List<GetCategoryResponseDTO>()
                    };
                }

                // ✅ Map entities → DTOs
                var responseDTOs = _mapper.Map<List<GetCategoryResponseDTO>>(categoryEntities);

                _logger.LogInformation("Successfully retrieved {Count} Asset Categories for TenantId: {TenantId}",
                    responseDTOs.Count, request.DTO.TenantId);

                // ✅ Return formatted API response
                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Asset Categories fetched successfully.",
                    Data = responseDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Asset Categories for TenantId: {TenantId}", request.DTO?.TenantId);
                return new ApiResponse<List<GetCategoryResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching asset categories.",
                    Data = null
                };
            }
        }
    }
}
