using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.PolicyType
{
    public class GetAllUnStructuredPolicyTypeRequestDTO
    {      

            public required int PolicyTypeId { get; set; }
            public required int PolicyTypeEnumVal { get; set; }           
            public required bool IsActive { get; set; }
            




    }
}
