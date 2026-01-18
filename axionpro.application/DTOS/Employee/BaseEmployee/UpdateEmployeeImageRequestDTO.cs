using axionpro.application.DTOS.Common;
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
        
        public long Id { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? FileName { get; set; }
        public required bool IsActive { get; set; }
        
        public ExtraPropRequestDTO? Prop = new ExtraPropRequestDTO();

    }
}
