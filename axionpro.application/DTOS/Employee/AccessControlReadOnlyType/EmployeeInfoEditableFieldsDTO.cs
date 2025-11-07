using axionpro.application.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Employee.AccessControlReadOnlyType
{
    public class EmployeeInfoEditableFieldsDTO
    {

        [AccessControl(ReadOnly = true)]
        public long TenantId { get; set; }

        [AccessControl(ReadOnly = true)]
        public int EmployeeDocumentId { get; set; }

        [AccessControl(ReadOnly = true)]

        public required string OfficialEmail { get; set; }

        [AccessControl(ReadOnly = true)]
        public required string EmployementCode { get; set; }
        [AccessControl(ReadOnly = false)]
        public required string FirstName { get; set; }
        [Required]
        [AccessControl(ReadOnly = false)]
        [MaxLength(100)]
        public string? MiddleName { get; set; }


        [AccessControl(ReadOnly = false)]

        public required string LastName { get; set; }

        [AccessControl(ReadOnly = true)]
        public DateTime? DateOfOnBoarding { get; set; }


        [AccessControl(ReadOnly = false)]
        public DateTime? DateOfBirth { get; set; }

        [AccessControl(ReadOnly = true)]
        public int? DepartmentId { get; set; }    // Optional Dropdown

        [AccessControl(ReadOnly = true)]
        public int? DesignationId { get; set; }   // Optional Dropdown

        [AccessControl(ReadOnly = true)]
        public required int EmployeeTypeId { get; set; }   // Dropdown: e.g., Intern, Full-Time, Contractor

        [AccessControl(ReadOnly = true)]
        public required bool HasPermanent { get; set; }    // Permanent employment flag


        [AccessControl(ReadOnly = true)]
        public required bool IsActive { get; set; }        // Active flag


    }

}
