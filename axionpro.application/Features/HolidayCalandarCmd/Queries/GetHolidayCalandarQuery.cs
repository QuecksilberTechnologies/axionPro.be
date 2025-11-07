using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.DTOs.Role;
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.HolidayCalandarCmd.Queries
{
    public class GetHolidayCalandarQuery : IRequest<ApiResponse<List<OrganizationHolidayCalendarDTO>>>
    {
        public BasicRequestDTO Dto { get; set; }

        public GetHolidayCalandarQuery(BasicRequestDTO dTO)
        {
            Dto = dTO;
        }
    }
}
