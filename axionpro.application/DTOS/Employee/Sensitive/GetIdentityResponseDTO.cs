using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Sensitive
{
    public class GetIdentityResponseDTO: BaseRequest
    {      
        public string? EmployeeId { get; set; }
        public string? AadhaarNumber { get; set; }
        public string? PanNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? DrivingLicenseNumber { get; set; }
        public string? VoterId { get; set; }
        public string? BloodGroup { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Nationality { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }        
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditable { get; set; }
        public bool IsVerified { get; set; }
        public   bool hasPanIdUploaded { get; set; }
        public bool hasAadharIdUploaded { get; set; }
        public bool hasPassportIdUploaded { get; set; }

        public string? passportDocPath { get; set; }
        public string? panDocPath { get; set; }
        public string? aadharDocPath { get; set; }
        public double? CompletionPercentage {  get; set; }
    }

}
