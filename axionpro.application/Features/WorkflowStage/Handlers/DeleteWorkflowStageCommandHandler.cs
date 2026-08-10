// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to Delete Workflow Stage.
// ================================================================

using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Features.WorkflowStage.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace axionpro.application.Features.WorkflowStage.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to Delete Workflow Stage.
    /// </summary>
public class DeleteWorkflowStageCommand : IRequest<ApiResponse<bool>>
    {

        public DeleteWorkflowStageRequestDTO? DTO { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWorkflowStageCommand"/> class.
        /// </summary>

        public DeleteWorkflowStageCommand(DeleteWorkflowStageRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

    #endregion
}

namespace axionpro.application.Features.WorkflowStage.Handlers
{
    /// <summary>
    /// Handles the request to Delete Workflow Stage.
    /// </summary>
public class DeleteWorkflowStageCommandHandler : IRequestHandler<DeleteWorkflowStageCommand, ApiResponse<bool>>
    {
        #region Fields

        private readonly IWorkflowStagesRepository _workflowRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<DeleteWorkflowStageCommandHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWorkflowStageCommandHandler"/> class.
        /// </summary>


        public DeleteWorkflowStageCommandHandler(
            IWorkflowStagesRepository workflowRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<DeleteWorkflowStageCommandHandler> logger)
        {
            _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied DeleteWorkflowStageCommand.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<bool>> Handle(DeleteWorkflowStageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.DTO == null || request.DTO.Id <= 0)
                    return new ApiResponse<bool>(false, "Invalid workflow stage Id.", false);

                // Entity fetch karo
                var entity = await _workflowRepository.GetByIdAsync(request.DTO.Id);
                if (entity == null)
                    return new ApiResponse<bool>(false, "Workflow stage not found.", false);

                // Delete repository call
                await _workflowRepository.DeleteAsync(request.DTO.Id, request.DTO.EmployeeId);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Workflow stage with Id {Id} deleted successfully by EmployeeId {EmpId}.",
                    request.DTO.Id, request.DTO.EmployeeId);

                return new ApiResponse<bool>(true, "Workflow stage deleted successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow stage with Id {Id}", request.DTO.Id);
                return new ApiResponse<bool>(false, $"Error deleting workflow stage: {ex.Message}", false);
            }
        }
    
        #endregion
}
}
