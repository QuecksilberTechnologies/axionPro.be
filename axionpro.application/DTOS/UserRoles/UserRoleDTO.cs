using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.DTOS.UserRoles 
{
    public class UserRoleDTO
    {  
       
        public string? RoleName { get; set; }
        public long UserRoleId { get; set; }
        public int RoleId { get; set; }   
        public bool IsActive { get; set; }             
        public bool IsPrimaryRole { get; set; }  =false;           
        public int RoleType { get; set; }
        
    
       
    } 
   public class UserRoleListDTO 
    {
        public long EmployeeId { get; set; } = 0;
        public List<UserRoleDTO> UserRoles { get; set; } = new List<UserRoleDTO>();
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();
    }


}
