using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class GetMinimalEmployeeResponseDTO
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string EmployementCode { get; set; } = null!;
        public int EmployeeTypeId { get; set; }
        public int CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? EmergencyContactPerson { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? BloodGroup { get; set; }
        public string? MobileNumber { get; set; }
        public int? Relation { get; set; }
        public string? Nationality { get; set; }
        public string? EmployeeTypeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public string? OfficialEmail { get; set; }
        public bool IsActive { get; set; }
        public bool IsSoftDeleted { get; set; }
        public bool? IsEditAllowed { get; set; }
        public bool HasPermanent { get; set; }
        public int GenderId { get; set; }
        public string? GenderName { get; set; }
        public string? Remark { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfOnBoarding { get; set; }
        public long? UpdateById { get; set; }
        public DateTime? UpdateByDateTime { get; set; }
       
      
    }

}
