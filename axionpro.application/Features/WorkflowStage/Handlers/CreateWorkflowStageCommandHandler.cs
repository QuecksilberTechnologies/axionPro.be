using AutoMapper;
 
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.Features.WorkflowStage.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.WorkflowStage.Handlers
{
    /// <summary>
    /// Handles the creation of workflow stages.
    /// </summary>
    public class CreateWorkflowStageCommandHandler: IRequestHandler<CreateWorkflowStageCommand, ApiResponse<List<GetWorkflowStageResponseDTO>>>
    {
        private readonly IWorkflowStagesRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRepository _commonRepository;

        public CreateWorkflowStageCommandHandler(
            IWorkflowStagesRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonRepository commonRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _commonRepository = commonRepository;
        }

        public async Task<ApiResponse<List<GetWorkflowStageResponseDTO>>> Handle(CreateWorkflowStageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate incoming request
                if (request.DTO == null)
                {
                    return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid workflow stage request. Data is required.",
                        Data = new List<GetWorkflowStageResponseDTO>()
                    };
                }

          

                // 3️⃣ Repository Call — Add workflow stage
                var response = await _unitOfWork.WorkflowStagesRepository.AddAsync(request.DTO);

                if (response == null || !response.Any())
                {
                    return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Workflow stage creation failed.",
                        Data = new List<GetWorkflowStageResponseDTO>()
                    };
                }

                // 4️⃣ Success response
                return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Workflow stage created successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                // 5️⃣ Exception handling with clear message
                return new ApiResponse<List<GetWorkflowStageResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while creating workflow stage: {ex.Message}",
                    Data = new List<GetWorkflowStageResponseDTO>()
                };
            }
        }
    }
}
