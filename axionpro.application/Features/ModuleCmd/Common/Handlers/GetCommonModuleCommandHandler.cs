using AutoMapper;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.Features.ModuleCmd.Common.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.ModuleCmd.Common.Handlers
{
    /// <summary>
    /// Handles retrieval of Common Modules.
    /// </summary>
    public class GetCommonModuleCommandHandler : IRequestHandler<GetCommonModuleRequestCommand, ApiResponse<List<GetCommonModuleResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCommonModuleCommandHandler> _logger;

        public GetCommonModuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetCommonModuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetCommonModuleResponseDTO>>> Handle(GetCommonModuleRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("❌ Invalid request received in GetCommonModuleCommandHandler.");
                    return new ApiResponse<List<GetCommonModuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                // ✅ Step 2: Call Repository to Get Common Modules
                List<GetCommonModuleResponseDTO>? commonModules = await _unitOfWork.ModuleRepository.GetCommonModuleAsync(request.DTO);

                // ✅ Step 3: Null or empty validation
                if (commonModules == null || commonModules.Count == 0)
                {
                    _logger.LogWarning("⚠️ No common module data found for the given request.");
                    return new ApiResponse<List<GetCommonModuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No common modules found.",
                        Data = null
                    };
                }

                // ✅ Step 4: Commit (optional for read ops, but safe for consistency)
                await _unitOfWork.CommitTransactionAsync();

                // ✅ Step 5: Log success
                _logger.LogInformation("✅ Common Modules fetched successfully. Total Modules: {Count}", commonModules.Count);

                // ✅ Step 6: Return success response
                return new ApiResponse<List<GetCommonModuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Common modules retrieved successfully.",
                    Data = commonModules
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching common modules.");
                return new ApiResponse<List<GetCommonModuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to retrieve common modules.",
                    Data = null
                };
            }
        }
    }
}
