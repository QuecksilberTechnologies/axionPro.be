using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class UpdateEmployeeImageRequestDTO
    {
        public long? _UserEmployeeId { get; set; }
        public long? _Id { get; set; }
        public string? UserEmployeeId { get; set; }       
        public string? Id { get; set; }

        public IFormFile? ProfileImage { get; set; }
        public string? FileName { get; set; }
        public bool? IsActive { get; set; }

    }
}
