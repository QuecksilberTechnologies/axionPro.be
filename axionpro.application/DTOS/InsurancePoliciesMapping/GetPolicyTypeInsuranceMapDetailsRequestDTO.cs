namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class GetPolicyTypeInsuranceMapDetailsRequestDTO
    {
      
        public int PolicyTypeId { get; set; }
        public int InsuranceTypeId { get; set; }       
        public bool IsActive { get; set; }         

    }

}
