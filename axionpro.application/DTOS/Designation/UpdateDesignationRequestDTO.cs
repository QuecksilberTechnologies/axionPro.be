using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Designation
{
    public class UpdateDesignationRequestDTO
    {

        public required string Id { get; set; }
        public string? UserEmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DesignationName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }

    }
}
