using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Token
{
    
        public class GetTokenInfoDTO
        {
        public string UserId { get; set; } = string.Empty;
        public string? EmployeeId { get; set; }  
        public int RoleId { get; set; }  
        public int GenderId { get; set; }  
        public bool HasPermanent { get; set; }  
        public string GenderName { get; set; }  
        public string? RoleTypeId { get; set; }    
        public string RoleTypeName { get; set; } = string.Empty;
        public string EmployeeTypeId { get; set; }    
        public string EmployeeTypeName { get; set; }    
        public string? TenantId { get; set; }    
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? Expiry { get; set; }
        public bool IsExpired { get; set; }
       // public List<string> Permissions { get; set; } = new();
        public string  TenantEncriptionKey { get; set; } = null!;
      


    }



}
