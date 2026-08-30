using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Common
{
   public class DeleteRequestDTO : axionpro.application.DTOs.BaseDTO.PermissionRequestDTO
    {    
        public long Id { get; set; }    
      
    
    }
}
