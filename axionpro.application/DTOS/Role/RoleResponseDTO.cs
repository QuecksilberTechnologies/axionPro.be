using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.Role
{
    public class RoleResponseDTO
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = null!;
        public string? Remark { get; set; }

        public bool IsActive { get; set; }

    
    }
}
