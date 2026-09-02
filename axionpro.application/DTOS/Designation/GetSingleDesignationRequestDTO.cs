using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.application.DTOs.BaseDTO; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Designation
{
    public class GetSingleDesignationRequestDTO : PermissionRequestDTO
    {
        public required string UsertId { get; set; }
        public required int Id { get; set; } 
     
    }
}
