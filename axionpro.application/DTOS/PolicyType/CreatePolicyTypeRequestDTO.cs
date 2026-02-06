
using axionpro.application.DTOS.Common;
using axionpro.application.Interfaces.IFileStorage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    public class CreatePolicyTypeRequestDTO
    {
      
       
        public required bool  IsActive { get; set; }// = null! ;     
        public required string PolicyName { get; set; }// = null! ;       
        public IFormFile? FormFile { get; set; }
        public required string Description { get; set; }
      //    public CreateCompanyPolicyDocumentRequestDTO?  policyDocumentRequestDTO { get; set; } = new CreateCompanyPolicyDocumentRequestDTO();
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();


    }
}
