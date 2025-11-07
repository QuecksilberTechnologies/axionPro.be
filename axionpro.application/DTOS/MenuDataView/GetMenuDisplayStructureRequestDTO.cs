using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.MenuDataView
{
    /// <summary> Get menu display structure/summary>
    public class GetMenuDisplayStructureRequestDTO
    {
     
            /// <summary> TenantId Required</summary>
            // ✅ Required Fields
        
            public long TenantId { get; set; }
            public long EmployeeId { get; set; }
            public long RoleId { get; set; }


        
    }
}
