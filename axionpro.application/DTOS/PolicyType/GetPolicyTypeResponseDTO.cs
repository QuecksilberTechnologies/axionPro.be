using axionpro.application.DTOS.PolicyTypeDocument;

namespace axionpro.application.DTOs.PolicyType
{
    public class GetPolicyTypeResponseDTO
    {
        public int Id { get; set; }
         
        public string PolicyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }   
       // public bool IsMappedWithInsurance { get; set; } = false;
        public bool IsStructured { get; set; }
        public int PolicyTypeEnumVal { get; set; }
        //   public List<GetPolicyTypeInsuranceMappingResponseDTO> InsuranceMappingList { get; set; }   = new List<GetPolicyTypeInsuranceMappingResponseDTO>();
        // 🔥 FIX: MULTIPLE EMPLOYEE TYPES
        public  List<int> EmployeeTypeIds { get; set; } = new();
        public List<GetPolicyTypeDocumentResponseDTO> DocDetails { get; set; } = new();

    }

}
