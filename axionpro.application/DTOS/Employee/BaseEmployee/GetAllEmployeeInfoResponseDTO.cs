using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public string? DateOfOnBoarding { get; set; }
            public int DepartmentId { get; set; }
            public string? DepartmentName { get; set; }
            public int DesignationId { get; set; }
            public string? DesignationName { get; set; }
            public int EmployeeTypeId { get; set; }
            public string? EmployeeTypeName { get; set; }
            public string? OfficialEmail { get; set; }          
            public bool IsActive { get; set; } = true;          
            public string? EmployeeImagePath { get; set; }           
         //   public   <List>AsignedAsset AsignedAssetList { get;set} 
            public double? CompletionPercentage { get; set; }
            public bool? HasImagePicUploaded { get; set; }
           
    }
    



}
 
