// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for processing Tenant Enabled Module.
// ================================================================

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
    /// <summary>
    /// Represents the TenantEnabledModuleRequestDTO data transfer model.
    /// </summary>
    public class TenantEnabledModuleRequestDTO
    {
       public bool IsActive { get; set; }
        

    }
}
