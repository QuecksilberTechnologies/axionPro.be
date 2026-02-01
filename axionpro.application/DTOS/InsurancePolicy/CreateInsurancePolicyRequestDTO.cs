using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    /// <summary>
    /// Create Insurance Policy request DTO
    /// (Audit fields are handled internally)
    /// </summary>
    public class CreateInsurancePolicyRequestDTO
    {
        // 🔹 Policy Classification
        public int PolicyTypeId { get; set; }          // FK → PolicyType
        public int? CountryId { get; set; }            // FK → Country

        // 🔹 Basic Policy Info
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

        // 🔹 Coverage Rules (CORE)
        public bool EmployeeAllowed { get; set; } = true;
        public int MaxSpouseAllowed { get; set; } = 0;
        public int MaxChildAllowed { get; set; } = 0;
        public bool ParentsAllowed { get; set; } = false;
        public bool InLawsAllowed { get; set; } = false;

        // 🔹 Status
        public bool IsActive { get; set; } = true;

        // 🔹 Additional Info
        public string? Remark { get; set; }
        public string? Description { get; set; }
    }
}

