
using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;

namespace axionpro.application.DTOs.PolicyType
{
    public class CreatePolicyTypeRequestDTO
    {
      
       
        public required bool  IsActive { get; set; }// = null! ;     
        public required bool IsStructured { get; set; } = true ;     
        public required string PolicyName { get; set; }// = null! ;
        public required int PolicyTypeEnumVal { get; set; }
        // 🔥 FIX: MULTIPLE EMPLOYEE TYPES
        public required List<int> EmployeeTypeIds { get; set; } = new();
        public IFormFile? FormFile { get; set; }
        public required string Description { get; set; }
      //    public CreateCompanyPolicyDocumentRequestDTO?  policyDocumentRequestDTO { get; set; } = new CreateCompanyPolicyDocumentRequestDTO();
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();


    }
}
