using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.SandwitchRule.DayCombination
{
    public class DeleteDayCombinationRequestDTO
    {
        public int Id { get; set; }
        public long? EmployeeId { get; set; }

        public long TenantId { get; set; }
 
       
    }
}
