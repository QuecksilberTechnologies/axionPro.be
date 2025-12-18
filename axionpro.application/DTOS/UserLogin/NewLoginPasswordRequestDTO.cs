using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserLogin
{
    public class NewLoginPasswordRequestDTO
    {
        public string Token { get; set; } = string.Empty;   // 🔐 Email se aaya hua token
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
