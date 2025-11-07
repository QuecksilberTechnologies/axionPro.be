using AutoMapper;
 
using axionpro.application.Features.AssetFeatures.Category.Commands;
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

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<DeleteCategoryCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Basic validation
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ DeleteStatusByTenantCommand or its DTO is null.");
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "❌ Invalid request. Data cannot be null.",
                        Data = false
                    };
                }

                if (request.DTO.TenantId <= 0 || request.DTO.Id <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid TenantId or StatusId received. TenantId: {TenantId}, Id: {Id}",
                        request.DTO.TenantId, request.DTO.Id);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "❌ Invalid TenantId or StatusId.",
                        Data = false
                    };
                }

                _logger.LogInformation("🗑️ Deleting AssetStatus Id: {Id} for TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.TenantId);

                // ✅ Step 2: Repository call
                bool isDeleted = await _unitOfWork.AssetCategoryRepository.DeleteAsync(request.DTO);

                // ✅ Step 3: Check result
                if (!isDeleted)
                {
                    _logger.LogWarning("⚠️ AssetStatus delete failed. Record not found. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id, request.DTO.TenantId);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "⚠️ Delete failed. Record not found or already deleted.",
                        Data = false
                    };
                }

                // ✅ Step 4: Return success
                _logger.LogInformation("✅ AssetStatus deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "✅ Asset Status deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting Asset Status. TenantId: {TenantId}, Id: {Id}",
                    request.DTO?.TenantId, request.DTO?.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"❌ Something went wrong while deleting Asset Status: {ex.Message}",
                    Data = false
                };
            }
        }
    }

}