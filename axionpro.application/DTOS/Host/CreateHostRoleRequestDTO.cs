using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.Host
{
    public class CreateHostRoleRequestDTO
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
