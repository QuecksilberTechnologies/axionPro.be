using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Common
{
    public class ExtraPropRequestDTO
    {
        public int RowId_int { get; set; }  
        public long RowId_long { get; set; } 
        public long UserEmployeeId { get; set; } 
        public long EmployeeId { get; set; }   
        public long TenantId { get; set; }
     

    }
}
