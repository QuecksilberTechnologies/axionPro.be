using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.CompletionPercentage
{
    public class EmployeeCompletionResponseDTO
    {
        public long EmployeeId { get; set; }
        public List<CompletionSectionDTO> Sections { get; set; } = new();
    }
}
