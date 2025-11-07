using axionpro.application.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Employee.AccessResponse
{
    public class GetEducationAccessResponseDTO
    {
        public FieldWithAccess<long?> Id { get; set; } = new();
        public FieldWithAccess<long?> EmployeeId { get; set; } = new();
        public FieldWithAccess<string?> Degree { get; set; } = new();
        public FieldWithAccess<string?> InstituteName { get; set; } = new();
        public FieldWithAccess<string?> Remark { get; set; } = new();
        public FieldWithAccess<DateTime?> StartDate { get; set; } = new();
        public FieldWithAccess<DateTime?> EndDate { get; set; } = new();
        public FieldWithAccess<string?> GradeOrPercentage { get; set; } = new();
        public FieldWithAccess<string?> GPAOrPercentage { get; set; } = new();
        public FieldWithAccess<bool?> EducationGap { get; set; } = new();
        public FieldWithAccess<string?> ReasonOfEducationGap { get; set; } = new();
        public FieldWithAccess<string?> EducationDocPath { get; set; } = new();
        public FieldWithAccess<int?> DocType { get; set; } = new();
        public FieldWithAccess<string?> DocName { get; set; } = new();
       public FieldWithAccess<long?> AddedById { get; set; } = new();
        public FieldWithAccess<DateTime?> AddedDateTime { get; set; } = new();
        public FieldWithAccess<long?> UpdatedById { get; set; } = new();
        public FieldWithAccess<DateTime?> UpdatedDateTime { get; set; } = new();
        public FieldWithAccess<long?> SoftDeletedById { get; set; } = new();
        public FieldWithAccess<DateTime?> DeletedDateTime { get; set; } = new();
        public FieldWithAccess<bool?> IsSoftDeleted { get; set; } = new();

        public FieldWithAccess<long?> InfoVerifiedById { get; set; } = new();
        public FieldWithAccess<bool?> IsInfoVerified { get; set; } = new();
        public FieldWithAccess<DateTime?> InfoVerifiedDateTime { get; set; } = new();
        public FieldWithAccess<bool?> IsEditAllowed { get; set; } = new();
        public FieldWithAccess<bool?> IsActive { get; set; } = new();
    }
}
