using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.domain.Entity
{
    public partial class CandidateDepartmentModuleSkill
    {
        public long Id { get; set; }

        public long CandidateId { get; set; }

        public long? CandidateDepartmentModuleSkill1 { get; set; }

        public DateTime? AddedDateTime { get; set; }

        public virtual Candidate Candidate { get; set; } = null!;

        public virtual CandidateDepartmentModuleSkill? CandidateDepartmentModuleSkill1Navigation { get; set; }

        public virtual ICollection<CandidateDepartmentModuleSkill> InverseCandidateDepartmentModuleSkill1Navigation { get; set; } = new List<CandidateDepartmentModuleSkill>();
    }

}
