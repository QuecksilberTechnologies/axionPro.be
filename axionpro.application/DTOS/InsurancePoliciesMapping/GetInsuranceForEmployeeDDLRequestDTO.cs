using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    public class GetInsuranceForEmployeeDDLRequestDTO
    {

        public int? EmployeeTypeId { get; set; }
        public bool? IsActive { get; set; } = true;
        public ExtraPropRequestDTO? Props { get; set; } = new ExtraPropRequestDTO();
    }
}
