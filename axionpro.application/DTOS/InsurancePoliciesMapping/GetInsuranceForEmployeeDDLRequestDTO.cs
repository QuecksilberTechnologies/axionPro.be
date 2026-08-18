// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Insurance For Employee DDL.
// ================================================================

using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.InsurancePoliciesMapping
{
    /// <summary>
    /// Represents the GetInsuranceForEmployeeDDLRequestDTO data transfer model.
    /// </summary>
    public class GetInsuranceForEmployeeDDLRequestDTO
    {

        public int? EmployeeTypeId { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
