using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class GetRoleSummaryResponseDTO
    {
        public long TenantId { get; set; }
        public bool IsActive { get; set; }
        public string RoleType { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public bool IsSystemDefault { get; set; }
    }
}
