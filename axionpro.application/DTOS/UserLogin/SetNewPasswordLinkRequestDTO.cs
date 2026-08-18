// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for processing Set New Password Link.
// ================================================================

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


    }
}
