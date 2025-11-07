using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.AssetTicketMap
{
    public class DeleteAssetTicketRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long? TenantId { get; set; }
        public long? Id { get; set; }   //filter by id if provided
         
             

        

    }
}
