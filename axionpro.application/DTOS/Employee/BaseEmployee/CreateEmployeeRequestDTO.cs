using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    /// <summary>
    /// Employee create request
    /// </summary>
    public class CreateBaseEmployeeRequestDTO : BaseRequest

    {     

        [Required]
        
        public int EmployeeDocumentId { get; set; }

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
        public int DesignationId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int? EmployeeTypeId { get; set; }

        [Required]
        public int? GenderId { get; set; }

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

        public int? RoleId { get; set; }

        public int? FunctionalId { get; set; }

        public int? ReferalId { get; set; }

        public string? Remark { get; set; }

        public string? Description { get; set; }

        public DateTime? InfoVerifiedDateTime { get; set; }

        public string? InfoVerifiedById { get; set; }

        public bool? IsInfoVerified { get; set; }

        public bool? IsDeleted { get; set; }

        public string? SoftDeletedById { get; set; }

        public DateTime? DeletedDateTime { get; set; }
        public string? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public bool IsEditAllowed { get; set; } = true;
       
    }


}
