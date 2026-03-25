using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace axionpro.application.DTOS.Employee.Experience
{
    public class GetExperienceRequestDTO: BaseRequest
    {
        // 🔹 Required (encoded)
        public string EmployeeId { get; set; } = string.Empty;

        // 🔹 Optional Filters
        public string? ExperienceId { get; set; }  // 🔥 specific record
        public bool? IsActive { get; set; }
        public bool? IsFresher { get; set; }       

        // 🔹 Common Props (Tenant/User context)
        public ExtraPropRequestDTO? Prop { get; set; } = new();
    }



}
