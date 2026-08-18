// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the Get All Insurance Policy Request With Employee Id DTO data transfer model.
// ================================================================

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



    }
}
