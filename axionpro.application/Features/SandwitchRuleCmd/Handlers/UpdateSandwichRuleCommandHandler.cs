// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to Update Sandwich Rule.
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
    /// Represents the request to Update Sandwich Rule.
    /// </summary>
public class UpdateSandwichRuleCommand : IRequest<ApiResponse<bool>>
    {

        public UpdateLeaveSandwitchRuleRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSandwichRuleCommand"/> class.
        /// </summary>

        public UpdateSandwichRuleCommand(UpdateLeaveSandwitchRuleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.SandwitchRuleCmd.Handlers
{
    /// <summary>
    /// Handles the request to Update Sandwich Rule.
    /// </summary>
public class UpdateSandwichRuleCommandHandler : IRequestHandler<UpdateSandwichRuleCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly ILogger<UpdateSandwichRuleCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSandwichRuleCommandHandler"/> class.
        /// </summary>


        public UpdateSandwichRuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISandwitchRuleRepository sandwitchRuleRepository,
            ILogger<UpdateSandwichRuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = sandwitchRuleRepository;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied UpdateSandwichRuleCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<bool>> Handle(UpdateSandwichRuleCommand request, CancellationToken cancellationToken)
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
                _logger.LogInformation("🚀 Updating Sandwich rule for TenantId: {TenantId}, Sandwich rule: {Id}", request.DTO.TenantId, request.DTO.Id);

                var result = await _unitOfWork.SandwitchRuleRepository.UpdateSandwichAsync(request.DTO);

                if (!result)
                {
                    _logger.LogWarning("⚠️ Sandwich rule update failed for TenantId: {TenantId}, Sandwich rule: {Id}", request.DTO.TenantId, request.DTO.Id);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "⚠️ Update failed. Record may not exist or no changes detected.",
                        Data = false
                    };
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Sandwich rule updated successfully for TenantId: {TenantId}, Sandwich rule: {Id}", request.DTO.TenantId, request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "✅ Sandwich rule updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ Error while updating Sandwich rule for TenantId: {TenantId}, Sandwich rule: {Id}", request.DTO.TenantId, request.DTO.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while updating Day Sandwich rule: {ex.Message}",
                    Data = false
                };
            }
        }
    
        #endregion
}
}
