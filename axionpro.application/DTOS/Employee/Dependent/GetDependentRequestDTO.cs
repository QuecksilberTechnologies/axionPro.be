using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Dependent
{
    public class GetDependentRequestDTO : BaseRequest
    {


        public required string UserEmployeeId { get; set; }
        public required string EmployeeId { get; set; }

        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

    }


}
