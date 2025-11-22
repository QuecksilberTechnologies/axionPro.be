using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Employee
{
    /// <summary>
    /// post-request to update any employe info :edu/basic,personal,bank,exp 
    /// </summary>

    public class GenericMultiFieldUpdateRequestDTO
    {
        [Required]
        [FromForm]
        public string UserEmployeeId { get; set; } = null!;

        public long _UserEmployeeId { get; set; }

        [Required]
        [FromForm]
        public string EmployeeId { get; set; } = null!;

        public long _EmployeeId { get; set; }

        [Required]
        [FromForm]
        public string Id { get; set; } = null!;

        public long _Id { get; set; }

        [Required]
        [FromForm]
        public string EntityName { get; set; } = string.Empty;

        // IMPORTANT 👇
        [FromForm]
        public List<FieldUpdateItemDTO> FieldsToUpdate { get; set; } = new();

          [FromForm]
        public List<FileUpdateItemDTO> FilesToUpdate { get; set; } = new();
    }

    public class FieldUpdateItemDTO
    {
        [FromForm]
        public string FieldName { get; set; } = string.Empty;

        [FromForm]
        public string? FieldValue { get; set; }

        [FromForm]
        public IFormFile? FileValue { get; set; }
    }

    public class FileUpdateItemDTO
    {
        [FromForm]
        public string? FieldName { get; set; } 

        [FromForm]
        public IFormFile? FieldValue { get; set; }
    }

}


