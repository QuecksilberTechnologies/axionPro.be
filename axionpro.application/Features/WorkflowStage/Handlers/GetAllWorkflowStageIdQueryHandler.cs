using AutoMapper;
 
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Features.WorkflowStage.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.WorkflowStage.Handlers
{
    /// <summary>
    /// Handles query to fetch all workflow stages.
    /// </summary>
    public class GetAllWorkflowStageQueryHandler : IRequestHandler<GetAllWorkflowStageQuery, ApiResponse<List<GetWorkflowStageResponseDTO>>>
    {
        private readonly IWorkflowStagesRepository _workflowRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<GetAllWorkflowStageQueryHandler> _logger;

        public GetAllWorkflowStageQueryHandler(
            IWorkflowStagesRepository workflowRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<GetAllWorkflowStageQueryHandler> logger)
        {
            _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the retrieval of all workflow stages based on filters.
        /// </summary>
        public async Task<ApiResponse<List<GetWorkflowStageResponseDTO>>> Handle(GetAllWorkflowStageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validation
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ Invalid request received in GetAllWorkflowStageQuery.");
                    return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. Please provide valid filter data.",
                        Data = new List<GetWorkflowStageResponseDTO>()
                    };
                }
 

                // 2️⃣ Repository Call
                var workflowStages = await _workflowRepository.AllAsync(request.DTO);

                // 3️⃣ Result Validation
                if (workflowStages == null || !workflowStages.Any())
                {
                   
                    return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No workflow stages found.",
                        Data = new List<GetWorkflowStageResponseDTO>()
                    };
                }

             

                return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Workflow stages fetched successfully.",
                    Data = workflowStages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching workflow stages.");
                return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while fetching workflow stages: {ex.Message}",
                    Data = new List<GetWorkflowStageResponseDTO>()
                };
            }
        }
    }
}
