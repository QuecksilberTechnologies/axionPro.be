using AutoMapper;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Handlers
{
    public class DeleteDayCombinationCommandHandler : IRequestHandler<DeleteDayCombinationCommand, ApiResponse<bool>>
    {
        private readonly ILogger<UpdateDayCombinationCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteDayCombinationCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISandwitchRuleRepository sandwitchRuleRepository,
            ILogger<UpdateDayCombinationCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = sandwitchRuleRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteDayCombinationCommand request, CancellationToken cancellationToken)
        {
            // Basic validation
            if (request.DTO == null)
            {
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "❌ Request data cannot be null.",
                    Data = false
                };
            }

            if (request.DTO.Id <= 0)
            {
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "❌ Invalid Combination Id.",
                    Data = false
                };
            }

            if (request.DTO.TenantId <= 0)
            {
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "❌ Invalid Tenant Id.",
                    Data = false
                };
            }

            try
            {
                _logger.LogInformation("🚀 Updating DayCombination for TenantId: {TenantId}, CombinationId: {Id}", request.DTO.TenantId, request.DTO.Id);

                var result = await _repository.DeleteDayCombinationAsync(request.DTO);

                if (!result)
                {
                    _logger.LogWarning("⚠️ DayCombination update failed for TenantId: {TenantId}, CombinationId: {Id}", request.DTO.TenantId, request.DTO.Id);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "⚠️ Update failed. Record may not exist or no changes detected.",
                        Data = false
                    };
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ DayCombination updated successfully for TenantId: {TenantId}, CombinationId: {Id}", request.DTO.TenantId, request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "✅ DayCombination updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ Error while updating DayCombination for TenantId: {TenantId}, CombinationId: {Id}", request.DTO.TenantId, request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while updating Day Combination: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
