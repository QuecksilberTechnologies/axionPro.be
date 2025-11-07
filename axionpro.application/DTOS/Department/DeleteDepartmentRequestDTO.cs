using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Department
{
    public class DeleteDepartmentRequestDTO

    {
        public required string Id { get; set; } // यूज़र ID जिसने ऐड किया

        public required string UserEmployeeId { get; set; }

    }
}

