using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.UserLogin
{
    public class ForgotPasswordResponseDTO
    {
        public int EmailResendAfterMinute  { get; set; }

        public string? Message { get; set; } 
    }
}
