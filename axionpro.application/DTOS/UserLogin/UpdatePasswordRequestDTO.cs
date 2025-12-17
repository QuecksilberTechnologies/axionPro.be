using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.UserLogin
{
    public class UpdatePasswordRequestDTO
    {      
        public required string LoginId { get; set; }          // User's login ID
        public required string UserEmployeeId { get; set; }          // User's login ID
        public required string EmployeeId { get; set; }          // User's login ID     
        public required string NewPassword { get; set; }         // User's password       
        public required string OldPassword { get; set; }         // User's password       
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
