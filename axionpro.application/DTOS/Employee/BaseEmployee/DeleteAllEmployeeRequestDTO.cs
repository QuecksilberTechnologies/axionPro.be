using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
    public class DeleteBaseEmployeeRequestDTO
    {
      
        
        public required string EmployeeId { get; set; } = string.Empty;
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();


    }

}
