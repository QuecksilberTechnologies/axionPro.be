using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.InsurancePolicy
{
    public class GetAllInsurancePolicyRequestDTO  
    {     

       public required int PolicyId { get; set; }
        public required bool  IsActive { get; set; }
        
    }


}


