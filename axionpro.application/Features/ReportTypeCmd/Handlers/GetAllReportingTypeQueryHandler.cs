using AutoMapper;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Features.ReportTypeCmd.Queries;
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

namespace axionpro.application.Features.ReportTypeCmd.Handlers
{
    /// <summary>
    /// Handles query to fetch all ReportingTypes.
    /// </summary>
    public class GetAllReportingTypeQueryHandler : IRequestHandler<GetAllReportingTypeQuery, ApiResponse<List<GetReportingTypeResponseDTO>>>
    {
        private readonly IReportingTypeRepository _workflowRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRepository _commonRepository;
        private readonly ILogger<GetAllReportingTypeQueryHandler> _logger;

        public GetAllReportingTypeQueryHandler(
            IReportingTypeRepository workflowRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICommonRepository commonRepository,
            ILogger<GetAllReportingTypeQueryHandler> logger)
        {
            _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _commonRepository = commonRepository ?? throw new ArgumentNullException(nameof(commonRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the retrieval of all ReportingTypes based on filters.
        /// </summary>
        public async Task<ApiResponse<List<GetReportingTypeResponseDTO>>> Handle(GetAllReportingTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validation
                if (request == null || request.DTO == null)
                {
                    _logger.LogWarning("⚠️ Invalid request received in GetAllReportingTypeQuery.");
                    return new ApiResponse<List<GetReportingTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request. Please provide valid filter data.",
                        Data = new List<GetReportingTypeResponseDTO>()
                    };
                }
 

                // 2️⃣ Repository Call
                var ReportingTypes = await _workflowRepository.AllAsync(request.DTO);

                // 3️⃣ Result Validation
                if (ReportingTypes == null || !ReportingTypes.Any())
                {
                   
                    return new ApiResponse<List<GetReportingTypeResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No ReportingTypes found.",
                        Data = new List<GetReportingTypeResponseDTO>()
                    };
                }

             

                return new ApiResponse<List<GetReportingTypeResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "ReportingTypes fetched successfully.",
                    Data = ReportingTypes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while fetching ReportingTypes.");
                return new ApiResponse<List<GetReportingTypeResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while fetching ReportingTypes: {ex.Message}",
                    Data = new List<GetReportingTypeResponseDTO>()
                };
            }
        }
    }
}
