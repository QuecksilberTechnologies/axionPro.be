using AutoMapper;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Features.SandwitchRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.SandwitchRuleCmd.Handlers
{
    public class GetSandwichRuleCommandHandler
        : IRequestHandler<GetSandwichRuleCommand, ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>>
    {
        private readonly ILogger<GetSandwichRuleCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSandwichRuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISandwitchRuleRepository sandwitchRuleRepository,
            ILogger<GetSandwichRuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = sandwitchRuleRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>> Handle(
            GetSandwichRuleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Basic validation
                if (request?.DTO == null || request.DTO.TenantId <= 0)
                {
                    _logger.LogWarning("⚠️ Invalid TenantId or null request received in GetSandwichRuleCommand.");
                    return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "❌ Invalid request or TenantId.",
                        Data = Enumerable.Empty<GetLeaveSandwitchRuleResponseDTO>()
                    };
                }

                _logger.LogInformation("🚀 Fetching Sandwich rules for TenantId: {TenantId}", request.DTO.TenantId);

                // ✅ Repository call
                var sandwichRules = await _unitOfWork.SandwitchRuleRepository.GetAllActiveSandwichRulesAsync(
                    request.DTO.TenantId, request.DTO.IsActive);

                if (sandwichRules == null || !sandwichRules.Any())
                {
                    _logger.LogWarning("⚠️ No Sandwich rules found for TenantId: {TenantId}", request.DTO.TenantId);
                    return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "⚠️ No records found.",
                        Data = Enumerable.Empty<GetLeaveSandwitchRuleResponseDTO>()
                    };
                }

           

                return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Sandwich rule(s) fetched successfully.",
                    Data = sandwichRules
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching Sandwich rules for TenantId: {TenantId}", request?.DTO?.TenantId);

                return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while fetching Sandwich rules: {ex.Message}",
                    Data = Enumerable.Empty<GetLeaveSandwitchRuleResponseDTO>()
                };
            }
        }
    }
}
