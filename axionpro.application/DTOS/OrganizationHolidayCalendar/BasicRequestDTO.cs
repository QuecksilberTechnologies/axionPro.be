using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.OrganizationHolidayCalendar
{
    public class BasicRequestDTO : axionpro.application.DTOs.BaseDTO.PermissionRequestDTO
    {
        public long? TenantLocationId { get; set; }

        public int? HolidayYear { get; set; }
    }
}
