using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOs.Tenant
{
    public class TenantEnabledOperationsRequestDTO
    {
       public required bool IsActive { get; set; }
       public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();
        

    }
}
