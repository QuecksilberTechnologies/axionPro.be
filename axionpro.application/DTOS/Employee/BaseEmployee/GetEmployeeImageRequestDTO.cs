using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.BaseEmployee
{
 
    public class GetEmployeeImageRequestDTO : BaseRequest
    {

        public string? EmployeeId { get; set; } 
        public bool IsActive { get; set; } =true;
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();


    }
}
