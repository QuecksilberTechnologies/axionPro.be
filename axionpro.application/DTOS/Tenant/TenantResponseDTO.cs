using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Tenant
{
    public class TenantResponseDTO
    {
        public long Id { get; set; }

        public string CompanyName { get; set; } = null!;

        public string? TenantCode { get; set; }

        public string CompanyEmailDomain { get; set; } = null!;

        public string TenantEmail { get; set; } = null!;

        public string? ContactPersonName { get; set; }

        public string? ContactNumber { get; set; }

        public int CountryId { get; set; }

        public bool IsVerified { get; set; }

        public bool IsActive { get; set; }
    }
}
