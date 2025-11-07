using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    public class DeletePolicyTypeDTO
    {
        public int Id { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
    }
}
