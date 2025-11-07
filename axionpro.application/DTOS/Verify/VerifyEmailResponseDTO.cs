using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Verify
{
    public class VerifyEmailResponseDTO
    {
        public long EmployeeId { get; set; }      
        public long? TenantId { get; set; }      
        public string UserId { get; set; }      
        public string Expiry { get; set; }
        public bool IsExpired { get; set; }
    }
}
