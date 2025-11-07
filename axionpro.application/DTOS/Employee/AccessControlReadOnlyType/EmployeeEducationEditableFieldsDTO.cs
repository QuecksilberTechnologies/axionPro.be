using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using axionpro.application.Common.Attributes;

namespace axionpro.application.DTOs.Employee.AccessControlReadOnlyType
{
    public class EmployeeEducationEditableFieldsDTO
    {
        [AccessControl(ReadOnly = true)]
        public int Id { get; set; }

        [AccessControl(ReadOnly = true)]
        public int EmployeeId { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? Degree { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? InstituteName { get; set; }

        [AccessControl(ReadOnly = false)]
        public int? DocType { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? DocName { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? EducationDocPath { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? Remark { get; set; }

        [AccessControl(ReadOnly = false)]
        public DateTime? StartDate { get; set; }

        [AccessControl(ReadOnly = false)]
        public DateTime? EndDate { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? GradeOrPercentage { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? GPAOrPercentage { get; set; }

        [AccessControl(ReadOnly = false)]
        public bool EducationGap { get; set; }

        [AccessControl(ReadOnly = false)]
        public string? ReasonOfEducationGap { get; set; }

        [AccessControl(ReadOnly = true)]
        public long? AddedById { get; set; }

        [AccessControl(ReadOnly = true)]
        public DateTime? AddedDateTime { get; set; }

        [AccessControl(ReadOnly = true)]
        public long? UpdatedById { get; set; }

        [AccessControl(ReadOnly = true)]
        public DateTime? UpdatedDateTime { get; set; }

        [AccessControl(ReadOnly = true)]
        public long? SoftDeletedById { get; set; }

        [AccessControl(ReadOnly = true)]
        public DateTime? DeletedDateTime { get; set; }

        [AccessControl(ReadOnly = true)]
        public bool IsSoftDeleted { get; set; }

        [AccessControl(ReadOnly = true)]
        public long? InfoVerifiedById { get; set; }

        [AccessControl(ReadOnly = true)]
        public bool IsInfoVerified { get; set; }

        [AccessControl(ReadOnly = true)]
        public DateTime? InfoVerifiedDateTime { get; set; }

        [AccessControl(ReadOnly = true)]
        public bool IsEditAllowed { get; set; }

        [AccessControl(ReadOnly = true)]
        public bool IsActive { get; set; }
    }
}

