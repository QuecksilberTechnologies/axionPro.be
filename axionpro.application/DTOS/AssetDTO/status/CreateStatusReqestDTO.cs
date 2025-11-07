using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.status
{
    public class CreateStatusRequestDTO
    {
        /// <summary> TenantId Required</summary>
        [Required]
        public long TenantId { get; set; }
        /// <summary> Employee Id Required</summary>
        [Required]
        public long EmployeeId { get; set; }
        /// <summary> TenantId Required</summary>
        public bool IsActive { get; set; }
        public string StatusName { get; set; } = null!;
        public string? ColorKey { get; set; } = null!;
        public string? Description { get; set; }


    }
}
