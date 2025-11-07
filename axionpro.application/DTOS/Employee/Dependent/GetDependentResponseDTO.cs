using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Dependent
{
    public class GetDependentResponseDTO: BaseRequest
    {


         public string? EmployeeId { get; set; }
        public int RoleId { get; set; }
        public string? DependentName { get; set; }
        public string? Relation { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsCoveredInPolicy { get; set; }
        public bool? IsMarried { get; set; }
        public string? Remark { get; set; }
        public string? Description { get; set; }

        // 🔹 Audit Fields
        public string? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }     

        // 🔹 Flags

        public bool? IsActive { get; set; }

        // 🔹 Info Verification
        public string? InfoVerifiedById { get; set; }
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }
    }


}
