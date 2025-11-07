using AutoMapper;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Features.ReportTypeCmd.Commands;
using axionpro.application.Features.SandwitchRuleCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ReportTypeCmd.Handlers
{
    /// <summary>
    /// Handles the creation of ReportingTypes.
    /// </summary>
    public class CreateReportingTypeCommandHandler: IRequestHandler<CreateReportingTypeCommand, ApiResponse<List<GetReportingTypeResponseDTO>>>
    {

      
       
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRepository _commonRepository;
        private readonly ILogger<CreateReportingTypeCommandHandler> _logger;

        public CreateReportingTypeCommandHandler(
           
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonRepository commonRepository,
            ILogger<CreateReportingTypeCommandHandler> logger)
        {
            
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<List<GetReportingTypeResponseDTO>>> Handle(CreateReportingTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate incoming request
                if (request.DTO == null)
                {
                    return new ApiResponse<List<GetReportingTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid ReportingType request. Data is required.",
                        Data = new List<GetReportingTypeResponseDTO>()
                    };
                }

          

                // 3️⃣ Repository Call — Add ReportingType
                var response = await _unitOfWork.ReportingTypeRepository.AddAsync(request.DTO);

                if (response == null || !response.Any())
                {
                    return new ApiResponse<List<GetReportingTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "ReportingType creation failed.",
                        Data = new List<GetReportingTypeResponseDTO>()
                    };
                }

                // 4️⃣ Success response
                return new ApiResponse<List<GetReportingTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "ReportingType created successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                // 5️⃣ Exception handling with clear message
                return new ApiResponse<List<GetReportingTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while creating ReportingType: {ex.Message}",
                    Data = new List<GetReportingTypeResponseDTO>()
                };
            }
        }
    }
}
