using AutoMapper;
using axionpro.application.Features.ReportTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.ReportTypeCmd.Handlers
{
 

    public class DeleteReportingTypeCommandHandler : IRequestHandler<DeleteReportingTypeCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<DeleteReportingTypeCommandHandler> _logger;

        public DeleteReportingTypeCommandHandler(

            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<DeleteReportingTypeCommandHandler> logger)
        {

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<ApiResponse<bool>> Handle(DeleteReportingTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.DTO == null || request.DTO.Id <= 0)
                    return new ApiResponse<bool>(false, "Invalid ReportingType Id.", false);

                // Entity fetch karo
                var entity = await _unitOfWork.WorkflowStagesRepository.GetByIdAsync(request.DTO.Id);
                if (entity == null)
                    return new ApiResponse<bool>(false, "ReportingType not found.", false);

                // Delete repository call
                await _unitOfWork.WorkflowStagesRepository.DeleteAsync(request.DTO.Id, request.DTO.EmployeeId);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("ReportingType with Id {Id} deleted successfully by EmployeeId {EmpId}.",
                    request.DTO.Id, request.DTO.EmployeeId);

                return new ApiResponse<bool>(true, "ReportingType deleted successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ReportingType with Id {Id}", request.DTO.Id);
                return new ApiResponse<bool>(false, $"Error deleting ReportingType: {ex.Message}", false);
            }
        }
    }
}
