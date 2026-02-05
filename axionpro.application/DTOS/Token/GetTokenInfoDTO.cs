using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Token
{

    public class GetTokenInfoDTO
    {
        // 🔐 Identity
        public string UserId { get; set; } = string.Empty;
        public string? EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        // 🏢 Tenant
        public string? TenantId { get; set; }
        public string? TenantEncriptionKey { get; set; } //= null!;

        // 👤 Role & Type
        public string RoleId { get; set; } = string.Empty;
        public string RoleTypeId { get; set; } = string.Empty;
        public string RoleTypeName { get; set; } = string.Empty;

        public string EmployeeTypeId { get; set; } = string.Empty;
        public string EmployeeTypeName { get; set; } = string.Empty;

        // 👥 Profile
        public string GenderId { get; set; } = string.Empty;
        public string GenderName { get; set; } = string.Empty;
        public bool HasPermanent { get; set; }

        // 🔐 Token Control
        public string TokenPurpose { get; set; } = "Auth";
        public DateTime IssuedAt { get; set; }
        public DateTime? Expiry { get; set; }
        public bool IsExpired { get; set; }
        public int TokenVersion { get; set; }
        public bool IsFirstLogin { get; set; }

        // 🌐 Context
        public string ClientType { get; set; } = "Web";
        // public List<string> Permissions { get; set; } = new();
    }


}
