using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.Features.AssetFeatures.Assets.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Assets.Handlers
{
    /// <summary>
    /// Handles the soft deletion of an Asset.
    /// </summary>
    public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteAssetCommandHandler> _logger;

        public DeleteAssetCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<DeleteAssetCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input
                if (request?.DTO == null || request.DTO.Id <= 0)
                {
                    _logger.LogWarning("DeleteAssetCommand called with invalid AssetId or null DTO.");
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid Asset Id.",
                        Data = false
                    };
                }

                // ✅ Step 2: Call Repository to soft delete asset
                int result = await _unitOfWork.AssetRepository.DeleteAssetAsync(request.DTO);

                // ✅ Step 3: Check repository result
                switch (result)
                {
                    case 0:
                        _logger.LogWarning("Asset with Id {AssetId} not found for TenantId {TenantId}.",
                            request.DTO.Id, request.DTO.TenantId);
                        return new ApiResponse<bool>
                        {
                            IsSucceeded = false,
                            Message = "Asset not found.",
                            Data = false
                        };

                    case -1:
                        _logger.LogWarning("Asset with Id {AssetId} is currently assigned and cannot be deleted.",
                            request.DTO.Id);
                        return new ApiResponse<bool>
                        {
                            IsSucceeded = false,
                            Message = "Asset is currently assigned and cannot be deleted.",
                            Data = false
                        };

                    case 1:
                        _logger.LogInformation("Asset soft deleted successfully with Id: {AssetId}", request.DTO.Id);
                        await _unitOfWork.CommitTransactionAsync();
                        return new ApiResponse<bool>
                        {
                            IsSucceeded = true,
                            Message = "Asset deleted successfully.",
                            Data = true
                        };

                    default:
                        _logger.LogError("Unexpected result while deleting Asset with Id: {AssetId}", request.DTO.Id);
                        return new ApiResponse<bool>
                        {
                            IsSucceeded = false,
                            Message = "Failed to delete Asset due to unexpected error.",
                            Data = false
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while soft deleting Asset with Id: {AssetId}", request.DTO.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Error occurred while soft deleting Asset.",
                    Data = false
                };
            }
        }
    }
}
