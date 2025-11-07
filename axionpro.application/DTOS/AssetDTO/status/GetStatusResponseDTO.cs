using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.status
{
    public class GetStatusResponseDTO : BaseRequest
    {
        public long Id { get; set; }
        public long? TenantId { get; set; }       
        public bool? IsActive { get; set; }
        public string? StatusName { get; set; }  
        public string? ColorKey { get; set; }  
        public string? Description { get; set; }
        public long? AddedById { get; set; }    
        public long? UpdateById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public DateTime? AddedDateTime { get; set; }
    }
}
