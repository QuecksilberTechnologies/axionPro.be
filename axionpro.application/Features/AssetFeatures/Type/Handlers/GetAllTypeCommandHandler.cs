using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Features.AssetFeatures.Type.Commands;
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

namespace axionpro.application.Features.AssetFeatures.Type.Handlers
{
    public class GetAllTypeCommandHandler : IRequestHandler<GetAllTypeCommand, ApiResponse<List<GetTypeResponseDTO>>>

    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllTypeCommandHandler> _logger;

        public GetAllTypeCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllTypeCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetTypeResponseDTO>>> Handle( GetAllTypeCommand request,   CancellationToken cancellationToken)
        {
            try
            {
                if (request.DTO == null || request.DTO.TenantId == null || request.DTO.TenantId <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid request received for GetAllAssetTypeByTenantCommand.");
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("Invalid request or TenantId.");
                }

                _logger.LogInformation("🚀 Fetching all Asset Types for TenantId: {TenantId}", request.DTO.TenantId);

                // Repository call to get list of Asset Types (with join to AssetCategory if needed)
                List<GetTypeResponseDTO> assetTypeList = await _unitOfWork.AssetTypeRepository.GetAllAsync(request.DTO);

                if (assetTypeList == null || !assetTypeList.Any())
                {
                    _logger.LogWarning("⚠️ No Asset Types found for TenantId: {TenantId}", request.DTO.TenantId);
                    return ApiResponse<List<GetTypeResponseDTO>>.Fail("No Asset Types found.");
                }

                _logger.LogInformation("✅ Fetched {Count} Asset Types for TenantId: {TenantId}", assetTypeList.Count, request.DTO.TenantId);

                return ApiResponse<List<GetTypeResponseDTO>>.Success(assetTypeList, "Asset Types fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching Asset Types for TenantId: {TenantId}", request.DTO.TenantId);
                return ApiResponse<List<GetTypeResponseDTO>>.Fail($"Error occurred: {ex.Message}");
            }
        }


    }

}