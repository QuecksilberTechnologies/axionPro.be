using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class ActivateAllEmployeeRequestDTO
    {
        public string? UserEmployeeId { get; set; }
        public string? EmployeeId { get; set; }
        public long? Id { get; set; }
        public required bool IsActive { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();
    }

}
