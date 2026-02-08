using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    public class DeletePolicyTypeDTO
    {
        public int PolicyId { get; set; }        
        public required bool IsActive { get; set; }        

        
    }
}
