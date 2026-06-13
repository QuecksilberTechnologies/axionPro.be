using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Compliances.ComplianceRule
{

    public class UpdateComplianceRuleRequestDTO
    {
        public int ComplianceTypeId { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

    }
}
