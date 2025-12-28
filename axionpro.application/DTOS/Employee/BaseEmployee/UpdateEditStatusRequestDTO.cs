using axionpro.application.DTOS.Common;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class UpdateEditStatusRequestDTO
    {
        [Required]
        public string UserEmployeeId { get; set; } = default!;

        [Required]
        public string EmployeeId { get; set; } = default!;

        [Required]
        public bool IsEditable { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
