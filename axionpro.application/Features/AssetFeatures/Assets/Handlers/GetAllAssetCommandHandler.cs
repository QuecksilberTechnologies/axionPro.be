using AutoMapper;
using axionpro.application.DTOS.AssetDTO.asset;
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

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    public class GetAllAssetCommand : IRequest<ApiResponse<List<GetAssetResponseDTO>>>
    {
        public GetAssetRequestDTO DTO { get; set; }

        public GetAllAssetCommand(GetAssetRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetAllAssetCommandHandler : IRequestHandler<GetAllAssetCommand, ApiResponse<List<GetAssetResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllAssetCommandHandler> _logger;

        public GetAllAssetCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAllAssetCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ApiResponse<List<GetAssetResponseDTO>>> Handle(GetAllAssetCommand request, CancellationToken cancellationToken)
        {
            // 🧩 Step 1: Validate input
            if (request?.DTO == null)
            {
                _logger.LogWarning("⚠️ GetAllAssetByTenantCommand received with null DTO.");
                return new ApiResponse<List<GetAssetResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Invalid request. DTO cannot be null.",
                    Data = new List<GetAssetResponseDTO>()
                };
            }

            if (request.DTO.TenantId <= 0)
            {
                _logger.LogWarning("⚠️ Invalid TenantId provided: {TenantId}", request.DTO.TenantId);
                return new ApiResponse<List<GetAssetResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Invalid TenantId provided.",
                    Data = new List<GetAssetResponseDTO>()
                };
            }
           


            try
            {
                // 🧩 Step 2: Fetch data from repository
                var assets = await _unitOfWork.AssetRepository.GetAssetsByFilterAsync(request.DTO);
                //    request.DTO.TenantId, isActiveFilter
                //);
               
                // 🧩 Step 3: Check if no assets found
                if (assets == null  || !assets.Any())
                {
                    _logger.LogInformation("ℹ️ No assets found for TenantId: {TenantId}", request.DTO.TenantId);
                    return new ApiResponse<List<GetAssetResponseDTO>>
                    {
                        IsSucceeded = true,
                        Message = "No assets found for this tenant.",
                        Data = new List<GetAssetResponseDTO>()
                    };
                }

                // 🧩 Step 4: Return success response
                  _logger.LogInformation("✅ Retrieved {Count} assets for TenantId: {TenantId}", assets.Count, request.DTO.TenantId);
                return new ApiResponse<List<GetAssetResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Assets fetched successfully.",
                    Data =  assets
                };
            }
            catch (Exception ex)
            {
                // 🧩 Step 5: Handle exception
                _logger.LogError(ex, "❌ Error occurred while fetching assets for TenantId: {TenantId}", request?.DTO?.TenantId);

                return new ApiResponse<List<GetAssetResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "An unexpected error occurred while fetching assets.",
                    Data = new List<GetAssetResponseDTO>()
                };
            }
        }



    }
}
