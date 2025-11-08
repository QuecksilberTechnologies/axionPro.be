using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{

    public class UpdateRoleRequestDTO
    {
        public required string Id { get; set; }
        public string? UserEmployeeId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleType { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }

    }

}
