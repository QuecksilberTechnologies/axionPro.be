using axionpro.application.DTOs.BasicAndRoleBaseMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserLogin
{
    public class AccessDetailResponseDTO
    {
        public long EmployeeId { get; set; }
        public int? ForPlatform { get; set; }
        public IEnumerable<BasicMenuDTO>? BasicMenus { get; set; } = new List<BasicMenuDTO>();
        public IEnumerable<UserRolesPermissionOnModuleDTO>? UserRolesPermissionOnModule { get; set; } = new List<UserRolesPermissionOnModuleDTO>();
    }

}
