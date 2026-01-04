using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Dependent
{
    public class GetDependentResponseDTO 
    {


        public required long Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? DependentName { get; set; }
        public string? Relation { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsCoveredInPolicy { get; set; }
        public bool? IsMarried { get; set; }
        public string? Remark { get; set; }
        public string? Description { get; set; }  

        // 🔹 Flags

        public bool? IsActive { get; set; }
        public bool HasProofUploaded { get; set; }
        public bool HasUploadedAll { get; set; }
        public   double   CompletionPercentage { get; set; }
        public string? FilePath { get; set; }

        // 🔹 Info Verification
        public string? InfoVerifiedById { get; set; }
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }

    }


}
