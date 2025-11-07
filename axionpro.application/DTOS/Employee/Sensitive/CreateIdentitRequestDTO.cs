using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Sensitive
{
    public class CreateIdentityRequestDTO:BaseRequest
    {
    
        public string EmployeeId { get; set; }
        public int RoleId { get; set; }        
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

        // 🔹 Flags
        public bool IsActive { get; set; }
        public bool? IsEditAllowed { get; set; }
   
        // 🔹 Audit Fields
        public string? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
   
     
        public DateTime? DeletedDateTime { get; set; }

        // 🔹 Info Verification
 
        public bool? IsInfoVerified { get; set; }
    }
}

 