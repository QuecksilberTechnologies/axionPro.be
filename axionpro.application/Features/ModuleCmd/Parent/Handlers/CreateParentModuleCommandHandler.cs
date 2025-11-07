using AutoMapper;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOS.Module.ParentModule;
 
using axionpro.application.Features.ModuleCmd.Parent.Commands;
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

namespace axionpro.application.Features.ModuleCmd.Parent.Handlers
{
    /// <summary>
    /// Handles the creation of a new Parent Module.
    /// </summary>
    public class CreateParentModuleCommandHandler : IRequestHandler<CreateParentModuleRequestCommand, ApiResponse<List<GetParentModuleResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateParentModuleCommandHandler> _logger;

        public CreateParentModuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateParentModuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetParentModuleResponseDTO>>> Handle(CreateParentModuleRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("❌ Invalid request received in CreateParentModuleCommandHandler.");
                    return new ApiResponse<List<GetParentModuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                // ✅ Step 2: Call Repository to Add Parent Module
                List<GetParentModuleResponseDTO>? parentDto = await _unitOfWork.ModuleRepository.AddParentModuleAsync(request.DTO);

                // ✅ Step 3: Null or empty validation
                if (parentDto == null || parentDto.Count == 0)
                {
                    _logger.LogWarning("⚠️ No parent module data returned after creation.");
                    return new ApiResponse<List<GetParentModuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Module created, but no data returned.",
                        Data = null
                    };
                }

                // ✅ Step 4: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // ✅ Step 5: Log success
                _logger.LogInformation("✅ Parent Module created successfully. Total Modules fetched: {Count}", parentDto.Count);

                // ✅ Step 6: Return success response
                return new ApiResponse<List<GetParentModuleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Module created successfully.",
                    Data = parentDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating parent module.");
                return new ApiResponse<List<GetParentModuleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to create module.",
                    Data = null
                };
            }
        }


    }
}
