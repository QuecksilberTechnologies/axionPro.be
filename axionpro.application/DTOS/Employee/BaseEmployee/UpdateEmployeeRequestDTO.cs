using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    /// <summary>
    /// Employee create request
    /// </summary>
    public class UpdateEmployeeRequestDTO
    {

        [Required]
        public string? UserEmployeeId { get; set; }

        public string Id { get; set; } = string.Empty;

        [MaxLength(100)]
      
        public string? FirstName { get; set; }

        [MaxLength(100)]
       
        public string? LastName { get; set; }

      
        public DateTime? DateOfBirth { get; set; }
         
        public string? GenderId { get; set; }     

        [MaxLength(300)]
        public string? Remark { get; set; }

        [MaxLength(100)]
        public string? MiddleName { get; set; }
      
        public string? Description { get; set; }

        public ExtraPropRequestDTO? Prop { get; set; }
    }

}
