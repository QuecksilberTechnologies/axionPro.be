using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
   public class GetInsurancePolicyResponseDTO
    {
        //

        // 🔹 Identity
        public int InsurancePolicyId { get; set; }      

        // 🔹 Policy Type
        public int PolicyTypeId { get; set; }
        public string PolicyTypeName { get; set; } = string.Empty;

        // 🔹 Policy Info
        public string InsurancePolicyName { get; set; } = string.Empty;
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public string? ProviderName { get; set; }

        // 🌍 Country
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }

        // 📅 Policy Duration
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // 🧑 Agent Info
        public string? AgentName { get; set; }
        public string? AgentContactNumber { get; set; }
        public string? AgentOfficeNumber { get; set; }

        // 🌍 Coverage Rules (IMPORTANT)
        public bool EmployeeAllowed { get; set; }
        public int MaxSpouseAllowed { get; set; }
        public int MaxChildAllowed { get; set; }
        public bool ParentsAllowed { get; set; }
        public bool InLawsAllowed { get; set; }

        // 🔘 Status
        public bool IsActive { get; set; }
        public bool IsSoftDeleted { get; set; }

        // 📝 Additional Info
        public string? Remark { get; set; }
        public string? Description { get; set; }
 
       
    }

}
