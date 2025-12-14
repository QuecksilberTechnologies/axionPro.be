using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using MediatR;
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

 
        public int? EmployeeDocumentId { get; set; }
       
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public int DesignationId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int EmployeeTypeId { get; set; }

        [Required]
        public int GenderId { get; set; }

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
 
        public int RoleId { get; set; }

        
        public int? ReferalId { get; set; }

        public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();


    }


}
