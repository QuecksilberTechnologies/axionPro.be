using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    public class UpdatePolicyTypeRequestDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }// = null! ;     
        public  string? PolicyName { get; set; }// = null! ;       
        public IFormFile? FormFile { get; set; }
        public  string? Description { get; set; }
        //  public CreateCompanyPolicyDocumentRequestDTO?  policyDocumentRequestDTO { get; set; } = new CreateCompanyPolicyDocumentRequestDTO();
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();     
        
       
    }
}
