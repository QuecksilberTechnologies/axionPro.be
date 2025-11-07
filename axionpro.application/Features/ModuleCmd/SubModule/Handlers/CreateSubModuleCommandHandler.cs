using AutoMapper;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.ModuleCmd.SubModule.Handlers
{
    /// <summary>
    /// Handles the creation of a new Sub-Module.
    /// </summary>
    public class CreateSubModuleCommandHandler : IRequestHandler<CreateSubModuleRequestCommand, ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSubModuleCommandHandler> _logger;

        public CreateSubModuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateSubModuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetModuleChildInversResponseDTO>>> Handle(CreateSubModuleRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("❌ Invalid request received in CreateSubModuleCommandHandler.");
                    return new ApiResponse<List<GetModuleChildInversResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                // ✅ Step 2: Call Repository to Add Sub-Module
                List<GetModuleChildInversResponseDTO>? subModuleList = await _unitOfWork.ModuleRepository.AddSubModuleAsync(request.DTO);

                // ✅ Step 3: Null or empty validation
                if (subModuleList == null || subModuleList.Count == 0)
                {
                    _logger.LogWarning("⚠️ No Sub-Module data returned after creation.");
                    return new ApiResponse<List<GetModuleChildInversResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Sub-Module created, but no data returned.",
                        Data = null
                    };
                }

                // ✅ Step 4: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // ✅ Step 5: Log success
                _logger.LogInformation("✅ Sub-Module created successfully. Total Modules fetched: {Count}", subModuleList.Count);

                // ✅ Step 6: Return success response
                return new ApiResponse<List<GetModuleChildInversResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Sub-Module created successfully.",
                    Data = subModuleList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating Sub-Module.");
                return new ApiResponse<List<GetModuleChildInversResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to create Sub-Module.",
                    Data = null
                };
            }
        }
    }
}
