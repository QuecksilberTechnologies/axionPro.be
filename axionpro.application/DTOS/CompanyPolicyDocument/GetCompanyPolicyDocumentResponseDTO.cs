using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.CompanyPolicyDocument
{
    public class GetCompanyPolicyDocumentResponseDTO
    {
        public int Id { get; set; }
        public int PolicyTypeId { get; set; }  
        public string? DocumentTitle { get; set; } = null!;   
        public string? FileName { get; set; } = null!;    
        public string? URL { get; set; } = null!;         

        public bool? IsActive { get; set; } = true;
    }
}
