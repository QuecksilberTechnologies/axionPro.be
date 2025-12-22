using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Sensitive
{
    public class GetIdentityRequestDTO : BaseRequest
    {

        public string? EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
