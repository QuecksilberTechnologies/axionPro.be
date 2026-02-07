using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.PolicyType
{
    public class GetAllPolicyTypeResponseDTO
    {
        public int Id { get; set; }
       
        public string PolicyName { get; set; } = string.Empty;
       

    }

}
