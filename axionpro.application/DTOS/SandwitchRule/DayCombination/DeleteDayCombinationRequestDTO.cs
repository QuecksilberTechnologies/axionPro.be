using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.SandwitchRule.DayCombination
{
    public class DeleteDayCombinationRequestDTO
    {
        public int Id { get; set; }
        public long? EmployeeId { get; set; }

        public long TenantId { get; set; }
 
       
    }
}
