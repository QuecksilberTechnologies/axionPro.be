using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.UserRoles
{
    public class GetUserRoleRequestDTO
    {
        public required string EmployeeId { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
