// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Day Combination.
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
using axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands
{
    #region Command

    /// <summary>
    /// Represents the read-only request to retrieve Get Day Combination.
    /// </summary>
public class GetDayCombinationCommand : IRequest<ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>>
    {

        public GetDayCombinationRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetDayCombinationCommand"/> class.
        /// </summary>

        public GetDayCombinationCommand(GetDayCombinationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion
}

namespace axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Handlers
{
    /// <summary>
    /// Handles the request to Get Day Combination.
    /// </summary>
public class GetDayCombinationCommandHandler : IRequestHandler<GetDayCombinationCommand, ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>>
    {
        #region Fields

   
        private readonly ILogger<GetDayCombinationCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetDayCombinationCommandHandler"/> class.
        /// </summary>

      
        public GetDayCombinationCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISandwitchRuleRepository sandwitchRuleRepository,
            ILogger<GetDayCombinationCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = sandwitchRuleRepository;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetDayCombinationCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>> Handle(
     GetDayCombinationCommand request,
     CancellationToken cancellationToken)
        {
            // 🧩 Step 1️⃣ - Basic validations (null / invalid)
            if (request == null || request.DTO == null)
            {
                _logger.LogWarning("⚠️ Invalid request: DTO is null.");
                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Invalid request data. Please provide valid DayCombination details."
                };
            }

            if (request.DTO.TenantId <= 0)
            {
                _logger.LogWarning("⚠️ Invalid TenantId provided: {TenantId}", request.DTO.TenantId);
                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "TenantId must be greater than zero."
                };
            }

            // 🧾 Optional: check for IsActive null (in case it's nullable in DTO)
            if (!request.DTO.IsActive)
            {
                _logger.LogWarning("⚠️ IsActive value missing for TenantId: {TenantId}", request.DTO.TenantId);
                request.DTO.IsActive = true; // default true
            }

            try
            {
                _logger.LogInformation("🚀 Fetching DayCombinations for TenantId: {TenantId}, IsActive: {IsActive}",
                    request.DTO.TenantId, request.DTO.IsActive);

                // 🧩 Step 2️⃣ - Begin transaction (UnitOfWork)
                await _unitOfWork.BeginTransactionAsync();

                // 🧩 Step 3️⃣ - Fetch active combinations
                var result = await _repository.GetAllActiveDayCombinationsAsync(request.DTO.TenantId, request.DTO.IsActive);

                // 🧩 Step 4️⃣ - Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ DayCombinations fetched successfully for TenantId: {TenantId}. Count: {Count}",
                    request.DTO.TenantId, result?.Count() ?? 0);

                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Day Combinations fetched successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // 🧩 Step 5️⃣ - Rollback in case of error
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ Error occurred while fetching DayCombinations for TenantId: {TenantId}", request.DTO.TenantId);

                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while fetching Day Combinations: {ex.Message}",
                    Data = new List<GetDayCombinationResponseDTO>()
                };
            }
        }


    
        #endregion
}
}
