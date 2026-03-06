using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.Entity
{
    public class GetEntityNameRequestDTO
    {
        public long TenantId { get; set; }    
       

    }
}
