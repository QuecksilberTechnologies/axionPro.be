using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    /// <summary>
    /// Employee create request
    /// </summary>
    public class UpdateEmployeeRequestOfficialDTO
    {
        [Required]
        public string UserEmployeeId { get; set; } = default!;
        public string EmployeeId { get; set; } = default!;

        // ---- Official / Admin controlled ----
        public int? DesignationId { get; set; }
        public bool? IsMarried { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? BloodGroup { get; set; }
        public string? SelfNumber { get; set; }
        public int? Relation { get; set; }
        public int? CountryId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeTypeId { get; set; }
        public int? GenderId { get; set; }      
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfOnBoarding { get; set; }
        public DateTime? DateOfExit { get; set; }
        public bool? HasPermanent { get; set; }
        public bool? IsEditAllowed { get; set; }
        public bool? IsInfoVerified { get; set; }
        public bool? IsActive { get; set; }
        public string? Remark { get; set; }
 
        public ExtraPropRequestDTO Prop { get; set; } = new();
    }

}
