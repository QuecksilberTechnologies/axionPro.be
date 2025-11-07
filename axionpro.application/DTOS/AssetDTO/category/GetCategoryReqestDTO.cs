using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.category
{
    public class GetCategoryReqestDTO 
    {
        public int Id { get; set; }
        public long TenantId { get; set; }
        public int RoleId { get; set; }       
        public bool? IsActive { get; set; }

    }
}
