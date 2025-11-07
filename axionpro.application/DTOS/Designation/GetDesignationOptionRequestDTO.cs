using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Designation
{
    public class GetDesignationOptionRequestDTO
    {
        public required string UserEmployeeId { get; set; }
        public required int DepartmentId { get; set; }       
        public bool IsActive { get; set; } = true;
    }
}
