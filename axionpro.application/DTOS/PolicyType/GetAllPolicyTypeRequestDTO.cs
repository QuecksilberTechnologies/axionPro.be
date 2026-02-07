
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IFileStorage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    // =====================================================
    // 🔹 Request DTO for fetching PolicyType list / grid
    // =====================================================
    public class GetAllPolicyTypeRequestDTO 
    {
       
        public required bool IsActive { get; set; }     

       
    }
}
 



