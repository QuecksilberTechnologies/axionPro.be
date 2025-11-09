using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    /// <summary>
    /// Employee create request
    /// </summary>
    public class CreateBaseEmployeeRequestDTO 

    {     

        [Required]
        public string UserEmployeeId { get; set; } 


        [Required]
        
        public string EmployeeDocumentId { get; set; }

       // [Required]
        [MaxLength(50)]
        public  string? EmployementCode { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string DesignationId { get; set; }

        [Required]
        public string DepartmentId { get; set; }

        [Required]
        public string? EmployeeTypeId { get; set; }

        [Required]
        public string? GenderId { get; set; }

        //[Required]
        public required bool HasPermanent { get; set; }

        //[Required]
        public required bool IsActive { get; set; }

        [Required]
        [EmailAddress]
        public required string OfficialEmail { get; set; }

        // 🔻 Optional Fields
        [MaxLength(100)]
        public string? MiddleName { get; set; }

        public DateTime? DateOfOnBoarding { get; set; }

        public DateTime? DateOfExit { get; set; }

        public string? RoleId { get; set; }

        public string? FunctionalId { get; set; }

        public string? ReferalId { get; set; }

        public string? Remark { get; set; }

        public string? Description { get; set; }

        public DateTime? InfoVerifiedDateTime { get; set; }

        public string? InfoVerifiedById { get; set; }

        public bool? IsInfoVerified { get; set; }
      public bool? IsEditAllowed { get; set; } 
       
    }


}
