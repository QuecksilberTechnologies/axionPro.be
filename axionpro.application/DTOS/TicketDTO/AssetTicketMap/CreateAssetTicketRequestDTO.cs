using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.AssetTicketMap
{
    public class CreateAssetTicketRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long? TenantId { get; set; }
        
        public int TicketTypeId { get; set; }   // Required - Linked with TicketType table

        public int AssetTypeId { get; set; }    // Required - Linked with AssetType table

        public int? ResponsibleRoleId { get; set; } // Optional - May or may not be assigned

        public bool IsActive { get; set; } = true;  // Default active when added

        // Soft delete info
        public bool? IsSoftDeleted { get; set; } = false;

        // Audit fields
        public long AddedById { get; set; }
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;

        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public long? SoftDeletedById { get; set; }
        public DateTime? SoftDeletedTime { get; set; }

       
    }
}
