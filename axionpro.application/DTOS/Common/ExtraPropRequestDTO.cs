using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Common
{
    public class ExtraPropRequestDTO
    {
      //  public int RowId_int { get; set; }  
        public long RowId { get; set; } 
        public bool IsActive { get; set; } = true;
        public long UserEmployeeId { get; set; } 
        public long EmployeeId { get; set; }   
        public long TenantId { get; set; }
     

    }
}
