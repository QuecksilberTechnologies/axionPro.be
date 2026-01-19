using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Tenant
{
    public class EmployeeCodePatternRequestDTO
    {
        public int? Id { get; set; }
        public required long TenantId { get; set; }
        public required bool IsActive { get; set; }
        
    }

}
