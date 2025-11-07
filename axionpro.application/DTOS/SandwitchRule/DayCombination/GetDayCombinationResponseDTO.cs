using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.SandwitchRule.DayCombination
{
    public class GetDayCombinationResponseDTO
    {
        public int Id { get; set; }

        public long? TenantId { get; set; }

        public string CombinationName { get; set; } = string.Empty;

        public int StartDay { get; set; }     // 1 = Monday ... 7 = Sunday

        public int EndDay { get; set; }

        public string? Remark { get; set; }

        public bool IsActive { get; set; }

        public bool IsSoftDeleted { get; set; }

        public long AddedById { get; set; }

        public DateTime AddedDateTime { get; set; }

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedDateTime { get; set; }
    }
}
