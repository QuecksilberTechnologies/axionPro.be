using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class GetInsurancePolicyRequestDTO : BaseRequest
    {
        public int? InsurancePolicyId { get; set; }
        public int? PolicyTypeId { get; set; }
        public string? InsurancePolicyName { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public string? ProviderName { get; set; }

        public int? CountryId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? AgentName { get; set; }
        public string? AgentContactNumber { get; set; }

        public bool? EmployeeAllowed { get; set; }
        public int? MaxSpouseAllowed { get; set; }
        public int? MaxChildAllowed { get; set; }
        public bool? ParentsAllowed { get; set; }
        public bool? InLawsAllowed { get; set; }

        public bool? IsActive { get; set; }

        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }


}


