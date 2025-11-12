
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Education
{
    public class GetEducationResponseDTO: BaseRequest
    {
         
       public string EmployeeId { get; set; } = string.Empty ;
        public string? Degree { get; set; }

        public string? InstituteName { get; set; }

        public string? Remark { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? GradeOrPercentage { get; set; }

        public string? GPAOrPercentage { get; set; }
        public string? EducationDocPath { get; set; }

        public int? DocType { get; set; }
        public string? DocName { get; set; }

        public bool? EducationGap { get; set; }

        public string? ReasonOfEducationGap { get; set; }

        public string? AddedById { get; set; }

        public DateTime? AddedDateTime { get; set; }

        public string? UpdatedById { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

 
        public string? InfoVerifiedById { get; set; }

        public bool HasEducationDocUploded { get; set; } 
        public bool? IsInfoVerified { get; set; } = false;

        public DateTime? InfoVerifiedDateTime { get; set; }

        public bool? IsEditAllowed { get; set; } = true;

        public bool? IsActive { get; set; }
    }


}
