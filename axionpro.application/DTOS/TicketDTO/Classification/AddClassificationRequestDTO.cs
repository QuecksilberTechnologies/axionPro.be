using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    public class AddClassificationRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long TenantId { get; set; }
        public string ClassificationName { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
       

     

 
    }
}
