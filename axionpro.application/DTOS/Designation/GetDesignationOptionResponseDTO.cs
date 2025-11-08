using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Designation
{
    public class GetDesignationOptionResponseDTO
    {
        public string? Id {  get; set; }
        public string? DepartmentId { get; set; }        
        public string? DesignationName { get; set; }       
       // public bool IsActive { get; set; }
    }
}
