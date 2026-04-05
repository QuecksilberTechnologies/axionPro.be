using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.PolicyType;

public class GetUnStructuredPolicyTypeResponseDTO
{

        public long Id { get; set; }

        public long TenantId { get; set; }
        public int EmployeeTypeId { get; set; }
        public string EmployeeTypeName { get; set; }
        
        public int PolicyTypeId { get; set; }
        public string PolicyTypeName { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }


    
    }






 
