// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for creating Compliance Rule.
// ================================================================

using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Compliances.ComplianceRule
{

    /// <summary>
    /// Represents the CreateComplianceRuleRequestDTO data transfer model.
    /// </summary>
    public class CreateComplianceRuleRequestDTO
    {
         
            public int ComplianceTypeId { get; set; }
            public int CountryId { get; set; }
            public int? StateId { get; set; }

            public object RuleJson { get; set; }

            public int Priority { get; set; }            

            public DateOnly EffectiveFrom { get; set; }   // 🔥 REQUIRED
         

    }
}
