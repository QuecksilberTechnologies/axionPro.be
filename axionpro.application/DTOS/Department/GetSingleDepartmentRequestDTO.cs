using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.application.DTOs.BaseDTO; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Department
{
    public class GetSingleDepartmentRequestDTO : PermissionRequestDTO
    {
        
        public required int Id { get; set; } 
     
    }
}
