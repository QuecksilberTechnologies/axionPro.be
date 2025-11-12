using axionpro.application.DTOS.Pagination;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Sensitive
{
    public class CreateIdentityRequestDTO:BaseRequest
    {
    
        public string UserEmployeeId { get; set; }
        public string EmployeeId { get; set; }      
        public string? AadhaarNumber { get; set; }
        public string? PanNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? DrivingLicenseNumber { get; set; }
        public string? VoterId { get; set; }
        public string? BloodGroup { get; set; }
        public bool? MaritalStatus { get; set; }
        public string? Nationality { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }   
        public IFormFile? AadhaarDocFile { get; set; }
        public IFormFile? PanDocFile { get; set; }
        public IFormFile? PassportDocFile { get; set; }
        
        // 🔹 Flags
      
     
    }
}

 