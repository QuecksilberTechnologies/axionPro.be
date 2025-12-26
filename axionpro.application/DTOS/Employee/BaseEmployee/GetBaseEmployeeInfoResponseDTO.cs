using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class GetBaseEmployeeResponseDTO
    {
                  
            public string? Id { get; set; }
            public string? EmployementCode { get; set; } 
            public string? LastName { get; set; } = string.Empty;
            public string? MiddleName { get; set; }
            public string? FirstName { get; set; } = string.Empty;
            public int GenderId { get; set; }
            public string? DesignationName { get; set; }
            public string? DepartmentName { get; set; }
            public string? EmergencyContactNumber { get; set; }
          public string? BloodGroup { get; set; }
        public string? SelfNumber { get; set; }
        public int? Relation { get; set; }
        public int? CountryId { get; set; }
            public string? Nationality { get; set; } = string.Empty;
            public string? RoleName { get; set; }
            public int? RoleId { get; set; }
            public int? RoleType { get; set; }
            public string? Type { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public DateTime? DateOfOnBoarding { get; set; }
            public DateTime? DateOfExit { get; set; }
            public int DesignationId { get; set; }
            public string? GenderName { get; set; }
            public int EmployeeTypeId { get; set; }
            public int DepartmentId { get; set; }
            public string? OfficialEmail { get; set; }
            public bool? HasPermanent { get; set; } = false;
            public bool IsActive { get; set; } = true;         
            public bool? IsEditAllowed { get; set; } 
            public bool? IsInfoVerified { get; set; }
        public double CompletionPercentage { get; set; }

    }
    



}
 
