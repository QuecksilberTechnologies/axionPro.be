using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Sensitive
{
    public class UpdateIdentityReqestDTO
    { 

            public required long Id { get; set; }
            public required string UserEmployeeId { get; set; }
            public required string EmployeeId { get; set; }
            public string? AadhaarNumber { get; set; }
            public string? PanNumber { get; set; }
            public string? PassportNumber { get; set; }
            public string? DrivingLicenseNumber { get; set; }
            public bool HasEPFAccount { get; set; }
            public string? UANNumber { get; set; }
            public string? VoterId { get; set; }
            public string? BloodGroup { get; set; }
            public bool? MaritalStatus { get; set; }
            public string? Nationality { get; set; }
            public string? EmergencyContactName { get; set; }
            public string? EmergencyContactRelation { get; set; }
            public string? EmergencyContactNumber { get; set; }
            public IFormFile? AadhaarDocFile { get; set; }
            public IFormFile? PanDocFile { get; set; }
            public IFormFile? PassportDocFile { get; set; }
            public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

            // 🔹 Flags


        }
   
}
