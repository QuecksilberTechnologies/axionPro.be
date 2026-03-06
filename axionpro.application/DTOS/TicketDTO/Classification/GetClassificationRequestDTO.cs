using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    public class GetClassificationRequestDTO 
    {

        public int? Id { get; set; }
        public long TenantId { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
    
         
 
    }
}
