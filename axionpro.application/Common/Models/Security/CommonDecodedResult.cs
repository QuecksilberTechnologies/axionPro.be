using axionpro.application.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Models.Security
{
    public class CommonDecodedResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public long LoggedInEmployeeId { get; set; }
        public long UserEmployeeId { get; set; }
        public long TenantId { get; set; }
        public int RoleId { get; set; }

        public TokenClaimsModel? Claims { get; set; }
    }
}
