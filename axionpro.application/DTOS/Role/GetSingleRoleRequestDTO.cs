using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Role
{
    public class GetSingleRoleRequestDTO
    {

        public required string UsertId { get; set; }
        public required int Id { get; set; }
    }
}
