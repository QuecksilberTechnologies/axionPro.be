using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.common
{
    public class GetAssetTypeByCategoryRequestDTO : BaseRequest
    {
        
        public int RoleId { get; set; }             
        public long? CategoryId { get; set; }      
        public bool? IsActive { get; set; }
        
    }
}
