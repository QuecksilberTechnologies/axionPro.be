using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Insurance
{
    public class CreateInsuranceRequestDTO
    {

        public long TenantId { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long InsurancePolicyId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }

        // 🔹 Approval Info
        public long? ApprovedById { get; set; }

        // 🔹 Status Flags
        public bool? IsActive { get; set; }
        public bool? IsSoftDeleted { get; set; }

        // 🔹 Audit Info
        public long? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public long? SoftDeletedById { get; set; }
        public DateTime? DeletedDateTime { get; set; }
    }


}
