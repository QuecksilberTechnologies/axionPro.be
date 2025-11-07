using AutoMapper;
using axionpro.application.Features.SandwitchRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.SandwitchRuleCmd.Handlers
{
    public class DeleteSandwichRuleCommandHandler : IRequestHandler<DeleteSandwichRuleCommand, ApiResponse<bool>>
    {
        private readonly ILogger<DeleteSandwichRuleCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteSandwichRuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISandwitchRuleRepository sandwitchRuleRepository,
            ILogger<DeleteSandwichRuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = sandwitchRuleRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteSandwichRuleCommand request, CancellationToken cancellationToken)
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
                    Message = "❌ Invalid Sandwich Rule Id.",
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
                _logger.LogInformation("🗑️ Deleting Sandwich rule for TenantId: {TenantId}, RuleId: {Id}", request.DTO.TenantId, request.DTO.Id);

                var result = await _unitOfWork.SandwitchRuleRepository.DeleteSandwichAsync(request.DTO);

                if (!result)
                {
                    _logger.LogWarning("⚠️ Sandwich rule delete failed for TenantId: {TenantId}, RuleId: {Id}", request.DTO.TenantId, request.DTO.Id);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "⚠️ Delete failed. Record may not exist or already deleted.",
                        Data = false
                    };
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Sandwich rule deleted successfully for TenantId: {TenantId}, RuleId: {Id}", request.DTO.TenantId, request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "✅ Sandwich rule deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ Error while deleting Sandwich rule for TenantId: {TenantId}, RuleId: {Id}", request.DTO.TenantId, request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while deleting Sandwich rule: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
