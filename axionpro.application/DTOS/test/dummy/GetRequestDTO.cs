using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.test.dummy
{
    public class GetRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }      
        public bool? IsActive { get; set; }
     
    }
}
