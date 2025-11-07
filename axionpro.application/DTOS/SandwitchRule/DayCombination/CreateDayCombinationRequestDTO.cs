using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.SandwitchRule.DayCombination
{
    public class CreateDayCombinationRequestDTO
    {

        public long EmployeeId { get; set; }

        public int RoleId { get; set; }

        public long TenantId { get; set; }

        public string CombinationName { get; set; } = string.Empty;

        public int StartDay { get; set; }     // 1 = Monday ... 7 = Sunday

        public int EndDay { get; set; }

        public string? Remark { get; set; }

        public bool IsActive { get; set; } = true;
       
      

  
    }
}
