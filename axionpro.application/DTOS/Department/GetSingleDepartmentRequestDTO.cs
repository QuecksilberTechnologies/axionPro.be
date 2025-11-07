using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Department
{
    public class GetSingleDepartmentRequestDTO
    {
        public required string UsertId { get; set; }
        public required int Id { get; set; } 
     
    }
}
