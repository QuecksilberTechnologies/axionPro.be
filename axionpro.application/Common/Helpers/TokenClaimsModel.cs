using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers
{
    public class TokenClaimsModel
    {
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public string? TenantEncriptionKey { get; set; }
        public string? RoleId { get; set; }
        public DateTime? Expiry { get; set; }
        public bool IsExpired { get; set; }
        // 🔥 ADD THESE
        public string? EmployeeId { get; set; }
        public string? TokenPurpose { get; set; }
        public string? Email { get; set; }
        



    }




}
