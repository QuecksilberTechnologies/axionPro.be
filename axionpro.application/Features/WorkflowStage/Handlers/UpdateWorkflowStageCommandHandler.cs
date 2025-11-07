using AutoMapper;
using axionpro.application.Constants;
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
    public class UpdateWorkflowStageCommandHandler : IRequestHandler<UpdateWorkflowStageCommand, ApiResponse<bool>>
    {
        private readonly IWorkflowStagesRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRepository _commonRepository;
        private readonly ILogger<UpdateWorkflowStageCommandHandler> _logger;

        public UpdateWorkflowStageCommandHandler(
            IWorkflowStagesRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonRepository commonRepository,
            ILogger<UpdateWorkflowStageCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<bool>> Handle(UpdateWorkflowStageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate Request
                if (request.DTO == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. WorkflowStage data is required.",
                        Data = false
                    };
                }

                if (request.DTO.Id <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "WorkflowStage Id must be valid.",
                        Data = false
                    };
                }

                // 2️⃣ Update WorkflowStage using Repository
                bool isUpdated = await _repository.UpdateAsync(request.DTO);

                if (!isUpdated)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "WorkflowStage update failed. Either not found or no changes detected.",
                        Data = false
                    };
                }

                // 3️⃣ Commit Transaction
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("WorkflowStage updated successfully with Id {Id}", request.DTO.Id);

                // 4️⃣ Return Success Response
                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "WorkflowStage updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating WorkflowStage with Id {Id}", request.DTO?.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while updating WorkflowStage: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
