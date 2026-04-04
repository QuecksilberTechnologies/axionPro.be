using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.DTOS.PolicyTypeDocument
{
    public class GetPolicyTypeDocumentResponseDTO
    {
        public long Id { get; set; }
        public int PolicyTypeId { get; set; }  
        public string? DocumentTitle { get; set; } = null!;   
        public string? FileName { get; set; }      
        public string? FilePath { get; set; } 
           

        public bool? IsActive { get; set; } = true;
    }
}
