using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserRole 
{
    public class UserRoleDTO
    {  
       
        public string RoleName { get; set; }
        public int RoleId { get; set; }   
        public bool IsActive { get; set; }      
        public bool IsPrimaryRole { get; set; }        
      
    
       
    } 

    
}
