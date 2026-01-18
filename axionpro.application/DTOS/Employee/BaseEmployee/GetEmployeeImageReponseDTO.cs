using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class GetEmployeeImageReponseDTO  //:  BaseRequest
    {
       
        public string? EmployeeId { get; set; } = null!;
        public long Id { get; set; } 
        public string? FileName { get; set; } = null!;
        public string? FilePath { get; set; }        
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public bool HasImageUploaded { get; set; }
        public double? CompletionPercentage { get; set; }




    }
}
