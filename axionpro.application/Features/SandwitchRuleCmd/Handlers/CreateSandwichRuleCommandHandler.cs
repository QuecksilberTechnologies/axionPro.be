using AutoMapper;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.Features.SandwitchRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.SandwitchRuleCmd.Handlers
{
    public class CreateSandwichRuleCommandHandler
        : IRequestHandler<CreateSandwichRuleCommand, ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>>
    {
        private readonly ILogger<CreateSandwichRuleCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSandwichRuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISandwitchRuleRepository sandwitchRuleRepository,
            ILogger<CreateSandwichRuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = sandwitchRuleRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>> Handle(
            CreateSandwichRuleCommand request,
            CancellationToken cancellationToken)
        {
            // ------------------ VALIDATION ------------------
            if (request.DTO == null)
            {
                return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ Request data cannot be null.",
                    Data = new List<GetLeaveSandwitchRuleResponseDTO>()
                };
            }

            if (string.IsNullOrWhiteSpace(request.DTO.RuleName))
            {
                return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ Rule name is required.",
                    Data = new List<GetLeaveSandwitchRuleResponseDTO>()
                };
            }

            if (request.DTO.TenantId <= 0)
            {
                return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ Invalid TenantId.",
                    Data = new List<GetLeaveSandwitchRuleResponseDTO>()
                };
            }

            try
            {
                _logger.LogInformation("🚀 Starting Sandwich Rule creation for TenantId: {TenantId}", request.DTO.TenantId);

                // ------------------ TRANSACTION START ------------------
                await _unitOfWork.BeginTransactionAsync();

                // 🔹 Add new Sandwich Rule
                var result = await _unitOfWork.SandwitchRuleRepository.AddSandwichAsync(request.DTO);

                // ------------------ COMMIT TRANSACTION ------------------
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Sandwich Rule created successfully for TenantId: {TenantId}", request.DTO.TenantId);

                return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Sandwich Rule added successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // ❌ Rollback on failure
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ Error while creating Sandwich Rule for TenantId: {TenantId}", request.DTO.TenantId);

                return new ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while creating Sandwich Rule: {ex.Message}",
                    Data = new List<GetLeaveSandwitchRuleResponseDTO>()
                };
            }
        }
    }
}
