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
    public class GetIdentityRequestDTO 
    {

        public required string EmployeeId { get; set; }
        public required string UserEmployeeId { get; set; }
        public required bool IsActive { get; set; }
        public required  int CountryNationalityId { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }
}
