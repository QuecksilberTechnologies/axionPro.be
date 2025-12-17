using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserLogin
{
    public class ResetLoginPasswordRequestDTO
    {
        public required string LoginId {  get; set; }
        public required string Otp { get; set; }
        public string Password { get; set; }

    }
}
