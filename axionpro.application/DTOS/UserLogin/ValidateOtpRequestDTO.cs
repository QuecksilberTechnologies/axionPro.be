using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserLogin
{
    public class ValidateOtpRequestDTO
    {
        public string LoginId { get; set; }
        public string? OTP { get; set; }
 
    }
}
