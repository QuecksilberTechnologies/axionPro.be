using AutoMapper;

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
    public class DeletetTypeCommandHandler : IRequestHandler<DeletetTypeCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletetTypeCommand> _logger;

        public DeletetTypeCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<DeletetTypeCommand> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeletetTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🗑️ Attempting to soft delete AssetType for TenantId: {TenantId}, Id: {Id}",
                    request.DTO.TenantId, request.DTO.Id);

                // ✅ Step 1: Call repository for soft delete
                bool isDeleted = await _unitOfWork.AssetTypeRepository.DeleteAsync(request.DTO);

                // ✅ Step 2: Handle result
                if (!isDeleted)
                {
                    _logger.LogWarning("⚠️ AssetType delete failed or not found. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id, request.DTO.TenantId);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Asset Type not found or already deleted.",
                        Data = false
                    };
                }

                // ✅ Step 3: Successful deletion response
                _logger.LogInformation("✅ AssetType deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Asset Type deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting Asset Type. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while deleting Asset Type: {ex.Message}",
                    Data = false
                };
            }
        }

    }
}
 