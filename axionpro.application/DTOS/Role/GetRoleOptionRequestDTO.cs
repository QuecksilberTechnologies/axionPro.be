using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Role
{
   
    public class GetRoleOptionRequestDTO
    {

        public required string UserEmployeeId { get; set; }
        public string? RoleType { get; set; } 
        public bool IsActive { get; set; } = true;

    }
}
