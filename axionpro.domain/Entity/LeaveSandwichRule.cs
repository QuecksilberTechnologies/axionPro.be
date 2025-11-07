using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.domain.Entity
{
    public partial class LeaveSandwichRule
    {
        public long Id { get; set; }

        public long TenantId { get; set; } 
    
       public string? RuleName { get; set; }

        public bool IsIncludeHoliday { get; set; }

        public bool IsIncludeWeekend { get; set; }

        public bool IsActive { get; set; }

        public string? Remark { get; set; }

        public long? AddedById { get; set; }

        public DateTime AddedDateTime { get; set; }

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedDateTime { get; set; }
        public DateTime? SoftDeletedDateTime { get; set; }

        public bool IsSoftDeleted { get; set; }

        public long? SoftDeletedById { get; set; }
        public virtual ICollection<LeaveSandwichRuleMapping> LeaveSandwichRuleMappings { get; set; } = new List<LeaveSandwichRuleMapping>();


    }

}
