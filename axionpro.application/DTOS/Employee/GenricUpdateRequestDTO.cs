using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOs.Employee
{
    /// <summary>
    /// post-request to update any employe info :edu/basic,personal,bank,exp 
    /// </summary>

    public class GenericMultiFieldUpdateRequestDTO
    {
        [Required]
         
        public string UserEmployeeId { get; set; } = null!;

        public long _UserEmployeeId { get; set; }

        [Required]
        
        public string EmployeeId { get; set; } = null!;

        public long _EmployeeId { get; set; }

        [Required]
         
        public string Id { get; set; } = null!;

        public long _Id { get; set; }

        [Required]
         
        public string EntityName { get; set; } = string.Empty;

        // IMPORTANT 👇
        
        public List<FieldUpdateItemDTO> FieldsToUpdate { get; set; } = new();

           
        public List<FileUpdateItemDTO> FilesToUpdate { get; set; } = new();
    }

    public class FieldUpdateItemDTO
    {
       
        public string FieldName { get; set; } = string.Empty;

         
        public string? FieldValue { get; set; }

         
        public IFormFile? FileValue { get; set; }
    }

    public class FileUpdateItemDTO
    {
         
        public string? FieldName { get; set; } 

        
        public IFormFile? FieldValue { get; set; }
    }

}


