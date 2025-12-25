using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.Features.ReportTypeCmd.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.ReportTypeCmd.Handlers
{
    public class UpdateReportingTypeCommandHandler : IRequestHandler<UpdateReportingTypeCommand, ApiResponse<bool>>
    {
        private readonly IReportingTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProcedureRepository _commonRepository;
        private readonly ILogger<UpdateReportingTypeCommandHandler> _logger;

        public UpdateReportingTypeCommandHandler(
            IReportingTypeRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IStoreProcedureRepository commonRepository,
            ILogger<UpdateReportingTypeCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<bool>> Handle(UpdateReportingTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate Request
                if (request.DTO == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. ReportingType data is required.",
                        Data = false
                    };
                }

                if (request.DTO.Id <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "ReportingType Id must be valid.",
                        Data = false
                    };
                }

                // 2️⃣ Update ReportingType using Repository
                bool isUpdated = await _repository.UpdateAsync(request.DTO);

                if (!isUpdated)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "ReportingType update failed. Either not found or no changes detected.",
                        Data = false
                    };
                }

                // 3️⃣ Commit Transaction
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("ReportingType updated successfully with Id {Id}", request.DTO.Id);

                // 4️⃣ Return Success Response
                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "ReportingType updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ReportingType with Id {Id}", request.DTO?.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while updating ReportingType: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
