using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave
{
    public class GetPolicyLeaveTypeByEmpTypeIdRequestDTO
    {
        public long TenantId { get; set; }                  // Required
        public long EmployeeId { get; set; } 
        public int EmployeeTypeId { get; set; }        
        public int RoleId { get; set; }                  // Required
        public bool IsActive { get; set; }              // Optional
        public DateTime TodaysDate { get; set; }               
         public bool IsMarriedApplicable { get; set; }
         public int ApplicableGenderId { get; set; }

    }
}
