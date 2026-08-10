// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the read-only request to retrieve Get Holiday Calandar.
// ================================================================

using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.DTOs.Role;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using axionpro.application.Features.HolidayCalandarCmd.Queries;
using axionpro.application.Interfaces;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.HolidayCalandarCmd.Queries
{
    #region Query

    /// <summary>
    /// Represents the read-only request to retrieve Get Holiday Calandar.
    /// </summary>
public class GetHolidayCalandarQuery : IRequest<ApiResponse<List<OrganizationHolidayCalendarDTO>>>
    {
        public BasicRequestDTO Dto { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHolidayCalandarQuery"/> class.
        /// </summary>

        public GetHolidayCalandarQuery(BasicRequestDTO dTO)
        {
            Dto = dTO;
        }
    }

    #endregion
}

namespace axionpro.application.Features.HolidayCalandarCmd.Handlers
{
    /// <summary>
    /// Handles the request to Get Holiday Calandar.
    /// </summary>
public class GetHolidayCalandarQueryHandler : IRequestHandler<GetHolidayCalandarQuery, ApiResponse<List<OrganizationHolidayCalendarDTO>>>
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetHolidayCalandarQueryHandler> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHolidayCalandarQueryHandler"/> class.
        /// </summary>


        public GetHolidayCalandarQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetHolidayCalandarQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region Handler
        /// <summary>
        /// Processes the supplied GetHolidayCalandarQuery.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="cancellationToken">The token used to observe cancellation.</param>
        /// <returns>The response produced for the request.</returns>


        public async Task<ApiResponse<List<OrganizationHolidayCalendarDTO>>> Handle(GetHolidayCalandarQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allHolidays = await _unitOfWork.HolidayCalandarRepository.GetAllHolidaysAsync();
                var holidayDTOs = _mapper.Map<List<OrganizationHolidayCalendarDTO>>(allHolidays);

                _logger.LogInformation("Successfully retrieved {Count} holidays.", holidayDTOs.Count);

                return new ApiResponse<List<OrganizationHolidayCalendarDTO>>
                {
                    IsSucceeded = true,
                    Message = "All holidays fetched.",
                    Data = holidayDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching holidays.");
                return new ApiResponse<List<OrganizationHolidayCalendarDTO>>
                {
                    IsSucceeded = false,
                    Message = "Error while fetching holiday calendar.",
                    Data = null
                };
            }
        }
    
        #endregion
}
}
