// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Parent Module.
// ================================================================

using axionpro.application.DTOs.Leave;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOS.Module.ParentModule;
using AutoMapper;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Command

    /// <summary>
    /// Represents the read-only request to retrieve Get Parent Module.
    /// </summary>
public class GetParentModuleCommand : IRequest<ApiResponse<List<GetParentModuleResponseDTO>>>
    {

        public GetParentModuleRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModuleCommand"/> class.
        /// </summary>

        public GetParentModuleCommand(GetParentModuleRequestDTO dTO)
        {
            DTO = dTO;
        }

    }

    #endregion
}

namespace axionpro.application.Features.ModuleCmd.Parent.Handlers
{
/// <summary>
    /// Handles the creation of a new Parent Module.
    /// </summary>
    public class GetParentModuleCommandHandler : IRequestHandler<GetParentModuleCommand, ApiResponse<List<GetParentModuleResponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetParentModuleCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModuleCommandHandler"/> class.
        /// </summary>


        public GetParentModuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetParentModuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetParentModuleCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<List<GetParentModuleResponseDTO>>> Handle(GetParentModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("❌ Invalid request received in GetParentModuleCommandHandler.");
                    return new ApiResponse<List<GetParentModuleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                // ✅ Step 2: Call Repository to Add Parent Module
                List<GetParentModuleResponseDTO>? parentDto = await _unitOfWork.ModuleRepository.GetParentModuleAsync(request.DTO);

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


    
        #endregion
}
}
