using axionpro.domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{

    public class RoleInfoDTO
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

    }

}
