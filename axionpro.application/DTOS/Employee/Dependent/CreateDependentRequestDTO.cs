using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Dependent
{
    public class CreateDependentRequestDTO
    {

         public string? UserEmployeeId { get; set; }
         public string EmployeeId { get; set; } = string.Empty;      
        public string? DependentName { get; set; }
        public string? Relation { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsCoveredInPolicy { get; set; }
        public bool? IsMarried { get; set; }
        public string? Remark { get; set; }
        public string? Description { get; set; }
        public bool HasProofUploaded {  get; set; }
      
        public IFormFile? ProofFile { get; set; }    




    }


}
