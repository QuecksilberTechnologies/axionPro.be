using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.UserLogin
{
    public class GetNewPasswordLinkResponseDTO
    {
         
        public string UrlLink { get; set; } = string.Empty;
  

    }
}
