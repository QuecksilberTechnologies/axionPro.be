using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class DeleteRoleRequestDTO
    {


        public required string Id { get; set; } // यूज़र ID जिसने ऐड किया

        public required string UserEmployeeId { get; set; }


    }
}
