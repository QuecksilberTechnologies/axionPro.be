using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.asset
{
    public class DeleteAssetReqestDTO
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public long? EmployeeId { get; set; }
        public int? RoleId { get; set; }
       
       
    }
}
