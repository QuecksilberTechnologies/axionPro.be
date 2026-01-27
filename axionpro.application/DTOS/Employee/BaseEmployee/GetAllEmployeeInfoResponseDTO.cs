using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.TicketFeatures.TicketHeader.Handlers;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class GetAllEmployeeInfoResponseDTO
    {
                  
            public string? EmployeeId { get; set; }
            public string? EmployementCode { get; set; } 
            public string? LastName { get; set; } = string.Empty;
            public string? MiddleName { get; set; }
             public string? FirstName { get; set; } = string.Empty;
            public int GenderId { get; set; }
            public string? GenderName { get; set; }
            public int CountryId { get; set; }
         public string? CountryCode { get; set; }
        public string? MobileNumber { get; set; }
          public string Nationality { get; set; } = string.Empty;           
            public string? DateOfBirth { get; set; }
            public string? DateOfOnBoarding { get; set; }
            public int DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public int DesignationId { get; set; }
            public string? DesignationName { get; set; }
            public int EmployeeTypeId { get; set; }
            public string? EmployeeTypeName { get; set; }
            public string? OfficialEmail { get; set; }          
            public string? EmergencyContactPerson { get; set; }          
            public bool IsActive { get; set; } = true;          
            public string? EmployeeImagePath { get; set; }           
         //   public   <List>AsignedAsset AsignedAssetList { get;set} 
            public double? CompletionPercentage { get; set; }
            public bool? HasImagePicUploaded { get; set; }
        //   public SummaryEmployeeInfo SummaryEmployeeInfo { get; set; } = new SummaryEmployeeInfo();



    }

  
 
    public class EmployeeProfileSummaryInfo
    {   
        public string? EmployeeId { get; set; }
        public string? FullName { get; set; }
        public int? Designation { get; set; }
        public string? DesignationType { get; set; }
        public int? Department { get; set; }
        public string? DepartmentType { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Mobile { get; set; }
        public string? MobileNumber { get; set; }
        public string? OffilcialEmail { get; set; }
        




    }
    public class SummaryEmployeeInfo
    {   
        public string? EmployeeId { get; set; }
        public string? EmergencyContactPerson { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? EmployeeCode { get; set; }
        public string? BloodGroup { get; set; }
        public string? MobileNumber { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CountryCode { get; set; }
        public int? Relation { get; set; }
        public string? RelationName  {  get; set;  }
        public bool IsActive { get; set; }
        public bool? IsMarried { get; set; }
        public int? OnlineStatus { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public int CurrentSalaryStatusId { get; set; }
        public string? CurrentSalaryStatusRemark { get; set; }
        public int RoleId { get; set; }
        public string? RoleType { get; set; }
        public int DesignationId { get; set; }
        public string? Designation { get; set; }
        public int DepartmentId { get; set; }
        public string? Department { get; set; }
        public string? ProfileImage { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public int EmployeeTypeId { get; set; }
        public string? EmployeeTypeName { get; set; }




    }

}
 
