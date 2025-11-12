using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Education
{
    public class CreateEducationRequestDTO
    {

        public string? UserEmployeeId { get; set; }
        public string? EmployeeId { get; set; }
        public string? Degree { get; set; }
        public string? InstituteName { get; set; }
        public string? Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? GradeOrPercentage { get; set; }
        public string? GPAOrPercentage { get; set; }
        public bool IsEducationGapBeforeDegree { get; set; }
        public bool HasEducationDocUploded { get; set; }
        public string? ReasonOfEducationGap { get; set; }              

        // 🔹 File related properties
        

        // 🔹 File itself
        public IFormFile? EducationDocument { get; set; }
    }


}
