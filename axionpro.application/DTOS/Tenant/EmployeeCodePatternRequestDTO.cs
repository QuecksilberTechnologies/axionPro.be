// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the mixed Tenant and Host employee-code pattern request contract.
// ================================================================

using System;
using axionpro.application.DTOs.BaseDTO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Tenant
{
    /// <summary>
    /// Requests an employee-code pattern. Tenant callers are scoped from their token; Host callers supply an encrypted Tenant identifier and permission pair.
    /// </summary>
    public class EmployeeCodePatternRequestDTO : PermissionRequestDTO
    {
        public int? Id { get; set; }
        /// <summary>Gets or sets the encrypted Tenant identifier for Host callers; it is ignored for Tenant callers.</summary>
        public string? TenantId { get; set; }
        public required bool IsActive { get; set; }
        
    }

}
