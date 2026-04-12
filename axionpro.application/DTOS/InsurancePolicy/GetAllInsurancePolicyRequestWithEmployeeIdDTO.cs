using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class GetAllInsurancePolicyRequestWithEmployeeIdDTO
    {
        public required string EmployeeId { get; set; }
        public required int PolicyId { get; set; }
        public required bool IsActive { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();



    }
}
