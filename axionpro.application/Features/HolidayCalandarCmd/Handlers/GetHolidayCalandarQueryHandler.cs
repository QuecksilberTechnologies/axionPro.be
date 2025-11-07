using AutoMapper;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.Features.HolidayCalandarCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.HolidayCalandarCmd.Handlers
{
    public class GetHolidayCalandarQueryHandler : IRequestHandler<GetHolidayCalandarQuery, ApiResponse<List<OrganizationHolidayCalendarDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetHolidayCalandarQueryHandler> _logger;

        public GetHolidayCalandarQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetHolidayCalandarQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

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
    }
}
