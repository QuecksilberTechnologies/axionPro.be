using AutoMapper;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using axionpro.application.Features.AssetFeatures.Type.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Type.Handlers
{
    public class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateAssetTypeCommandHandler> _logger;

        public UpdateAssetTypeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateAssetTypeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateAssetTypeCommand request, CancellationToken cancellationToken)
        {
            if (request?.DTO == null)
            {
                _logger.LogWarning("⚠️ UpdateAssetTypeCommand called with null DTO.");
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Request data cannot be null.",
                    Data = false
                };
            }

            try
            {
                _logger.LogInformation("🔄 Updating AssetType for TenantId: {TenantId}, Id: {Id}",
                   request.DTO.TenantId, request.DTO.Id);

                bool isUpdated = await _unitOfWork.AssetTypeRepository.UpdateAsync(request.DTO);

                if (!isUpdated)
                {
                    _logger.LogWarning("⚠️ AssetType not found or update failed. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id, request.DTO.TenantId);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Asset type not found or update failed.",
                        Data = false
                    };
                }

                _logger.LogInformation("✅ AssetType updated successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Asset type updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating AssetType. TenantId: {TenantId}, Id: {Id}",
                    request.DTO.TenantId, request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while updating asset type: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
