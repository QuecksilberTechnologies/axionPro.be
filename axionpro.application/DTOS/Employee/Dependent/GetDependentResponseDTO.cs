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

        // 🔥 ENUM VALUE (LOGIC / CALCULATION)
        public int? Relation { get; set; }

        // 🔥 ENUM NAME (DISPLAY ONLY)
        public string? RelationType { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public bool? IsCoveredInPolicy { get; set; }
        public bool? IsMarried { get; set; }

        public string? Remark { get; set; }
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
        public bool HasProofUploaded { get; set; }
        public bool HasUploadedAll { get; set; }

        public double CompletionPercentage { get; set; }
        public string? FilePath { get; set; }

        public string? InfoVerifiedById { get; set; }
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }
    }


}
