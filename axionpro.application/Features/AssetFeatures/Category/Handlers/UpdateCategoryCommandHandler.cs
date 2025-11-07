using AutoMapper;
using axionpro.application.Features.AssetFeatures.Category.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    /// <summary>
    /// Handles the update operation for Asset Category.
    /// </summary>
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<UpdateCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles update request for Asset Category.
        /// </summary>
        public async Task<ApiResponse<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            bool isUpdated = false;
            try
            {

                _logger.LogInformation("Updating Asset Category ID: {Id} for TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.TenantId);

                // ✅ Validation check
                if (request.DTO == null || request.DTO.Id <= 0)
                {
                    _logger.LogWarning("Invalid update request received. DTO is null or ID is missing.");
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = isUpdated,
                        Message = "Invalid request. Category ID is required.",
                        Data = isUpdated
                    };
                }

                // ✅ Call repository update
                var updatedCategory = await _unitOfWork.AssetCategoryRepository.UpdateAsync(request.DTO);

                if (!updatedCategory)
                {
                    _logger.LogWarning("No category found with ID: {Id}", request.DTO.Id);
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = isUpdated,
                        Message = $"No category found with ID {request.DTO.Id}.",
                        Data = isUpdated
                    };
                }
 

                _logger.LogInformation("✅ Asset Category updated successfully. ID: {Id}", request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Asset Category updated successfully.",
                    Data = isUpdated
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating asset category ID: {Id}", request.DTO.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while updating asset category.",
                    Data = false
                };
            }
        }
    }
}
