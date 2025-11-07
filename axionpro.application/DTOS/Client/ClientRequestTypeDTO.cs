using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Client
{
    public class ClientRequestTypeDTO
    {
        public long Id { get; set; }
        public int SelfRoleId { get; set; }
        public int DesignationId { get; set; }
        public int ClientTypeId { get; set; } //  
        public bool IsActive { get; set; } = true;

    }

}
