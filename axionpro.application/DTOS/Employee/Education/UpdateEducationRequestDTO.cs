using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Education
{
    public class UpdateEducationRequestDTO
    {
        public string UserEmployeeId { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string? Degree { get; set; }= string.Empty;
        public string? InstituteName { get; set; } = string.Empty;
        public string? Remark { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; }= null;
        public string? ScoreValue { get; set; } = string.Empty;
        public string? GradeDivision { get; set; } = string.Empty;
        public string? ScoreType { get; set; } = string.Empty;
        public bool? IsEducationGapBeforeDegree { get; set; } 
        public double GapYears { get; set; } = 0;
        public bool? HasEducationDocUploded { get; set; }
        public string? ReasonOfEducationGap { get; set; }
        public IFormFile? EducationDocument { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
