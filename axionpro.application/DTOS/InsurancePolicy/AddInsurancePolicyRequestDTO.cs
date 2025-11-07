using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    /// <summary>
    /// post-request get self profile info
    /// </summary>

    public class AddInsurancePolicyRequestDTO
    {

        public long TenantId { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public string InsurancePolicyName { get; set; } = string.Empty;
        public string InsurancePolicyNumber { get; set; } = string.Empty;
        public string? CoverageType { get; set; }
        public string? ProviderName { get; set; }

        // 🔹 Policy Duration
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // 🔹 Agent Info
        public string? AgentName { get; set; }
        public string? AgentContactNumber { get; set; }
        public string? AgentOfficeNumber { get; set; }

        // 🔹 Status Flags
        public bool? IsActive { get; set; }
        public bool? IsSoftDeleted { get; set; }

        // 🔹 Additional Info
        public string? Remark { get; set; }
        public string? Description { get; set; }

        // 🔹 Audit Info
        public long? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public long? SoftDeletedById { get; set; }
        public DateTime? DeletedDateTime { get; set; }
    }
}
