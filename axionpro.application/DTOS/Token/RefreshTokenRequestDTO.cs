using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Token
{
    namespace ems.application.DTOs.UserLogin
    {
        public class RefreshTokenRequestDTO
        {
          

            /// <summary>
            /// Client ka Refresh Token
            /// </summary>
            public string RefreshToken { get; set; } = string.Empty;

            /// <summary>
            /// Client ka IP Address (optional)
            /// </summary>
            public string? IpAddress { get; set; }
        }
    }

}
