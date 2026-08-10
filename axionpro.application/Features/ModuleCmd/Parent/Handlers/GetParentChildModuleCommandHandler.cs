// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Parent Child Module.
// ================================================================

using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Command

    /// <summary>
    /// Represents the read-only request to retrieve Get Parent Child Module.
    /// </summary>
public class GetParentChildModuleCommand : IRequest<ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {

        public GetModuleChildInversRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentChildModuleCommand"/> class.
        /// </summary>

        public GetParentChildModuleCommand(GetModuleChildInversRequestDTO dTO)
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
    public class GetParentChildModuleCommandHandler : IRequestHandler<GetParentChildModuleCommand, ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetParentChildModuleCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentChildModuleCommandHandler"/> class.
        /// </summary>


        public GetParentChildModuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetParentChildModuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetParentChildModuleCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<List<GetModuleChildInversResponseDTO>>> Handle(GetParentChildModuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate input request
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("❌ Invalid request received in GetAllModuleWithChildHeader.");
                    return new ApiResponse<List<GetModuleChildInversResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request data.",
                        Data = null
                    };
                }

                // ✅ Step 2: Call Repository to Add Parent Module
                List<GetModuleChildInversResponseDTO>? parentDto = await _unitOfWork.ModuleRepository.GetAllModuleTreeAsync();

                // ✅ Step 3: Null or empty validation
                if (parentDto == null || parentDto.Count == 0)
                {
                    _logger.LogWarning("⚠️ No parent module data returned after creation.");
                    return new ApiResponse<List<GetModuleChildInversResponseDTO>>
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
                return new ApiResponse<List<GetModuleChildInversResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Module created successfully.",
                    Data = parentDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating parent module.");
                return new ApiResponse<List<GetModuleChildInversResponseDTO>>
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
