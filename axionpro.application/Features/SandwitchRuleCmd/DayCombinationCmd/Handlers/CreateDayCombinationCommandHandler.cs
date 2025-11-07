using AutoMapper;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.SandwitchRuleCmd.DayCombinationCmd.Handlers
{
    public class CreateDayCombinationCommandHandler : IRequestHandler<CreateDayCombinationCommand, ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>>
    {
   
        private readonly ILogger<CreateDayCombinationCommandHandler> _logger;
        private readonly ISandwitchRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
      
        public CreateDayCombinationCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISandwitchRuleRepository sandwitchRuleRepository,
            ILogger<CreateDayCombinationCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = sandwitchRuleRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>> Handle(CreateDayCombinationCommand request, CancellationToken cancellationToken)
        {
            // Validation phase
            if (request.DTO == null)
            {
                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ Request data cannot be null.",
                    Data = new List<GetDayCombinationResponseDTO>()
                };
            }

            if (string.IsNullOrWhiteSpace(request.DTO.CombinationName))
            {
                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ Combination name is required.",
                    Data = new List<GetDayCombinationResponseDTO>()
                };
            }

            if (request.DTO.StartDay < 1 || request.DTO.StartDay > 7 ||
                request.DTO.EndDay < 1 || request.DTO.EndDay > 7)
            {
                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ StartDay and EndDay must be between 1 (Monday) and 7 (Sunday).",
                    Data = new List<GetDayCombinationResponseDTO>()
                };
            }

            if (request.DTO.TenantId <= 0)
            {
                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "❌ Invalid TenantId.",
                    Data = new List<GetDayCombinationResponseDTO>()
                };
            }

            try
            {
                _logger.LogInformation("🚀 Starting DayCombination creation for TenantId: {TenantId}", request.DTO.TenantId);

               

                // Add new record
                var result = await _repository.AddDayCombinationAsync(request.DTO);

             
                _logger.LogInformation("✅ DayCombination created successfully for TenantId: {TenantId}", request.DTO.TenantId);

                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "✅ Day Combination added successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // Rollback if any exception occurs
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ Error while creating DayCombination for TenantId: {TenantId}", request.DTO.TenantId);

                return new ApiResponse<IEnumerable<GetDayCombinationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"❌ Error while creating Day Combination: {ex.Message}",
                    Data = new List<GetDayCombinationResponseDTO>()
                };
            }
        }


    }
}
