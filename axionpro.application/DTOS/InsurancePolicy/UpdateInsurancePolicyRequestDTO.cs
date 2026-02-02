using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class UpdateInsurancePolicyRequestDTO
    {
        // 🔑 Identity (MANDATORY)
        [Required]
        public int Id { get; set; }

        // 🔹 Policy Classification        
        public int? PolicyTypeId { get; set; }
        public int? CountryId { get; set; }
      
        [Required]
        public string InsurancePolicyName { get; set; } = string.Empty;
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public string? ProviderName { get; set; }

        // 🔹 Policy Duration
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // 🔹 Agent Info
        public string? AgentName { get; set; }
        public string? AgentContactNumber { get; set; }
        public string? AgentOfficeNumber { get; set; }

        // 🔹 Coverage Rules
        // 🔹 Coverage Rules (UPDATE-SAFE)
        public bool? EmployeeAllowed { get; set; }
        public int? MaxSpouseAllowed { get; set; }
        public int? MaxChildAllowed { get; set; }
        public bool? ParentsAllowed { get; set; }
        public bool? InLawsAllowed { get; set; }


        // 🔹 Status
        public bool IsActive { get; set; }

        // 🔹 Additional Info
        public string? Remark { get; set; }
        public string? Description { get; set; }
    }
}
 

