// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to Update Day Combination.
// ================================================================

using axionpro.application.DTOs.Leave;
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
using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to Update Day Combination.
    /// </summary>
public class UpdateDayCombinationCommand : IRequest<ApiResponse<bool>>
    {

        public UpdateDayCombinationRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDayCombinationCommand"/> class.
        /// </summary>

        public UpdateDayCombinationCommand(UpdateDayCombinationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Handlers
{
    /// <summary>
    /// Handles the request to Update Day Combination.
    /// </summary>
public class UpdateDayCombinationCommandHandler : IRequestHandler<UpdateDayCombinationCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly ILogger<UpdateDayCombinationCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDayCombinationCommandHandler"/> class.
        /// </summary>


        public UpdateDayCombinationCommandHandler(
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
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied UpdateDayCombinationCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<bool>> Handle(UpdateDayCombinationCommand request, CancellationToken cancellationToken)
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

                var result = await _repository.UpdateDayCombinationAsync(request.DTO);

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
    
        #endregion
}
}
