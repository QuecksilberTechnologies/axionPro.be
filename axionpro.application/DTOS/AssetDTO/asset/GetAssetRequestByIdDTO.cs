using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.AssetDTO.asset
{
    public class GetAssetRequestByIdDTO
    {
        public long? TenantId { get; set; }
        public int RoleId { get; set; }      
        public long EmployeeId { get; set; }
        public long Id { get; set; }
        
        
    }
}
