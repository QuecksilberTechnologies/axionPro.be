
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Education
{
    public class GetEducationResponseDTO
    {

        public string? Id { get; set; }

        public string EmployeeId { get; set; } = string.Empty ;
        public string? Degree { get; set; }

        public string? InstituteName { get; set; }

        public string? Remark { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? GradeOrPercentage { get; set; }

        public string? GPAOrPercentage { get; set; }
        public string? FilecPath { get; set; }

        public string? FileType { get; set; }   
        public string? FileName { get; set; }   

        public bool? EducationGap { get; set; }

        public string? ReasonOfEducationGap { get; set; }
 
        public string? InfoVerifiedById { get; set; }

        public bool HasEducationDocUploded { get; set; } 
   
        public bool? IsActive { get; set; }
        public double? CompletionPercentage { get; set; }

    }


}
