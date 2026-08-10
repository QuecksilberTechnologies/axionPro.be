// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Processes the CreateSubModuleRequestCommand use case.
// ================================================================

using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Features.ModuleCmd.Parent.Handlers;
using axionpro.application.Wrappers;
using AutoMapper;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

using MediatR;

namespace axionpro.application.Features.ModuleCmd.SubModule.Commands
{
    #region Command

    /// <summary>
    /// Represents the command request for Create Sub Module Request.
    /// </summary>
public class CreateSubModuleRequestCommand : IRequest<ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {

        public CreateSubModuleRequestDTO DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSubModuleRequestCommand"/> class.
        /// </summary>

        public CreateSubModuleRequestCommand(CreateSubModuleRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }

    #endregion
}

namespace axionpro.application.Features.ModuleCmd.SubModule.Handlers
{
/// <summary>
    /// Handles the creation of a new Sub-Module.
    /// </summary>
    public class CreateSubModuleCommandHandler : IRequestHandler<CreateSubModuleRequestCommand, ApiResponse<List<GetModuleChildInversResponseDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSubModuleCommandHandler> _logger;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSubModuleCommandHandler"/> class.
        /// </summary>

        public CreateSubModuleCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateSubModuleCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region Handler

        /// <summary>
        /// Handles the request asynchronously.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The response produced by handling the request.</returns>

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
    
        #endregion
}
}
