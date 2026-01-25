using axionpro.application.DTOS.Common;
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
       
        public string? EmployeeId { get; set; }
        public string? Degree { get; set; }
        public string? InstituteName { get; set; }
        public string? Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ScoreValue { get; set; }
        public string? GradeDivision { get; set; }
        public string? ScoreType { get; set; }
        public bool IsEducationGapBeforeDegree { get; set; }
        public double GapYears { get; set; } = 0;
        public bool HasEducationDocUploded { get; set; }
        public string? ReasonOfEducationGap { get; set; }              

        public IFormFile? EducationDocument { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }


}
