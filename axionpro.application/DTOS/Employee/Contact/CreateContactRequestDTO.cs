using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Contact
{
    public class CreateContactRequestDTO
    {

        public long TenantId { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }

        // 🔹 Contact Info
        public int? ContactType { get; set; }          // e.g. Personal, Work, Emergency
        public string ContactNumber { get; set; } = string.Empty;
        public string? AlternateNumber { get; set; }
        public string? Email { get; set; }
        public bool? IsPrimary { get; set; }

        // 🔹 Address Info
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public string? HouseNo { get; set; }
        public string? LandMark { get; set; }
        public string? Street { get; set; }
        public string? LocalAddress { get; set; }
        public string? PermanentAddress { get; set; }

        // 🔹 Optional/Metadata
        public string? Remark { get; set; }
        public string? Description { get; set; }

        // 🔹 Status Flags
        public bool? IsActive { get; set; } = true;
        public bool? IsSoftDeleted { get; set; } = false;
        public bool? IsEditAllowed { get; set; } = true;
        public bool? IsInfoVerified { get; set; } = false;

        // 🔹 Audit
        public long AddedById { get; set; }
        public DateTime AddedDateTime { get; set; } = DateTime.Now;
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public long? SoftDeletedById { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public long? InfoVerifiedById { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }
    }
}
