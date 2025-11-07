using AutoMapper;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.Features.ModuleCmd.Common.Commands;
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

namespace axionpro.application.Features.ModuleCmd.Common.Handlers
{
    /// <summary>
    /// Handles the creation of a new Common Module.
    /// </summary>
    public class CreateCommonModuleCommandHandler : IRequestHandler<CreateCommonModuleCommand, ApiResponse<List<GetCommonModuleResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCommonModuleCommandHandler> _logger;

        public CreateCommonModuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateCommonModuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetCommonModuleResponseDTO>>> Handle(CreateCommonModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("❌ Invalid request received in CreatecommonModuleCommandHandler.");
                    return new ApiResponse<List<GetCommonModuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                // ✅ Step 2: Call Repository to Add Common Module
                List<GetCommonModuleResponseDTO>? commonDto = await _unitOfWork.ModuleRepository.AddCommonModuleAsync(request.DTO);

                // ✅ Step 3: Null or empty validation
                if (commonDto == null || commonDto.Count == 0)
                {
                    _logger.LogWarning("⚠️ No Common module data returned after creation.");
                    return new ApiResponse<List<GetCommonModuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Module created, but no data returned.",
                        Data = null
                    };
                }

                // ✅ Step 4: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // ✅ Step 5: Log success
                _logger.LogInformation("✅ Common Module created successfully. Total Modules fetched: {Count}", commonDto.Count);

                // ✅ Step 6: Return success response
                return new ApiResponse<List<GetCommonModuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Module created successfully.",
                    Data = commonDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating Common module.");
                return new ApiResponse<List<GetCommonModuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to create module.",
                    Data = null
                };
            }
        }


    }
}
