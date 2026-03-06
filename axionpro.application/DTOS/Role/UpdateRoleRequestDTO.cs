using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.Role
{

    public class UpdateRoleRequestDTO
    {
    
        public string? UserEmployeeId { get; set; }
        public required int Id { get; set; }
        public string? RoleName { get; set; }
        public int RoleType { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();


    }

}
