using AutoMapper;
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Features.WorkflowStage.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.WorkflowStage.Handlers
{
    public class DeleteWorkflowStageCommandHandler : IRequestHandler<DeleteWorkflowStageCommand, ApiResponse<bool>>
    {
        private readonly IWorkflowStagesRepository _workflowRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRepository _commonRepository;
        private readonly ILogger<DeleteWorkflowStageCommandHandler> _logger;

        public DeleteWorkflowStageCommandHandler(
            IWorkflowStagesRepository workflowRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonRepository commonRepository,
            ILogger<DeleteWorkflowStageCommandHandler> logger)
        {
            _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
    }
}
