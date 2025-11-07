
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    public class CreatePolicyTypeRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long TenantId { get; set; }
        public bool? IsActive { get; set; }// = null! ;     
        public string? PolicyName { get; set; }// = null! ;
        public string? Description { get; set; }
 
    }
}
