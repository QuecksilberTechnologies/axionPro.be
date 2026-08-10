// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Sandwich Rule.
// ================================================================

using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Features.SandwitchRuleCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace axionpro.application.Features.SandwitchRuleCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the read-only request to retrieve Get Sandwich Rule.
    /// </summary>
public class GetSandwichRuleCommand : IRequest<ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>>
    {

        public GetLeaveSandwitchRuleRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSandwichRuleCommand"/> class.
        /// </summary>

        public GetSandwichRuleCommand(GetLeaveSandwitchRuleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.SandwitchRuleCmd.Handlers
{
    /// <summary>
    /// Handles the request to Get Sandwich Rule.
    /// </summary>
public class GetSandwichRuleCommandHandler
        : IRequestHandler<GetSandwichRuleCommand, ApiResponse<IEnumerable<GetLeaveSandwitchRuleResponseDTO>>>
    {
        #region Fields

        private readonly ILogger<GetSandwichRuleCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSandwichRuleCommandHandler"/> class.
        /// </summary>


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
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetSandwichRuleCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


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
    
        #endregion
}
}
