using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.CompletionPercentage
{

    public class EmployeeProfileCompletionDTO
    {
        public CompletionSectionDTO Bank { get; set; }
        public CompletionSectionDTO Education { get; set; }
        public CompletionSectionDTO Experience { get; set; }
    }


    public class CompletionSectionDTO
    {
        public string? SectionName { get; set; }   // Bank, Contact, Experience...
        public double? CompletionPercent { get; set; }       
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
        public bool IsSectionCreate { get; set; }
    }


    public class EducationRowDTO
    {
        public string? Degree { get; set; }
        public string? InstituteName { get; set; }
        public int? ScoreType { get; set; }
        public bool? HasEducationDocUploded { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsInfoVerified { get; set; }
        public bool IsEditAllowed { get; set; }
    }

}
