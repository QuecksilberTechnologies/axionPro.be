using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Module
{
    public class TenantEnabledOperationRequestDTO
    {
       
          
            public long? TenantId { get; set; }

            public int ModuleId { get; set; }

            public int OperationId { get; set; }

            public bool IsOperationUsed { get; set; }
            public bool IsEnabled { get; set; }

            public long? AddedById { get; set; }

            public DateTime? AddedDateTime { get; set; }

            public long? UpdatedById { get; set; }

            public DateTime? UpdatedDateTime { get; set; }

    
       

    }
}
