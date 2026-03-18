using axionpro.application.DTOS.Common;
using axionpro.domain.Entity; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace axionpro.application.DTOs.UserLogin
{
    public class SetNewPasswordLinkRequestDTO 
    {
         
        public string UserLoginId { get; set; } = string.Empty;
        public ExtraPropRequestDTO? Prop { get; set; }


    }
}
