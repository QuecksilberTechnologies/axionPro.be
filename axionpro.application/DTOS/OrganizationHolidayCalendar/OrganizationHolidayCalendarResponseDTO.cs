using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.OrganizationHolidayCalendar
{
    public class OrganizationHolidayCalendarDTO
    {
      
        public long TenantId { get; set; }       
        public string? StateCode { get; set; }                   // e.g., "MH"
        public int HolidayYear { get; set; }                     // e.g., 2025
        public string HolidayName { get; set; } = string.Empty;
        public DateTime HolidayDate { get; set; }
        
 
    }

}
