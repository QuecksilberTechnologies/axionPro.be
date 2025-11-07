using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    public class GetClassificationResponseDTO : BaseRequest
    {

        public string ClassificationName { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsSoftDeleted { get; set; }

        public long? AddedById { get; set; }

        public DateTime? AddedDateTime { get; set; }

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

        // ✅ Add these two lines
     

    }
}
