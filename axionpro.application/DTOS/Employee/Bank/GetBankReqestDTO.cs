using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Bank
{
    public class GetBankReqestDTO: BaseRequest
    {
        
        public string? EmployeeId { get; set; } // e.g., Savings, Current, etc.
        public string? AccountType { get; set; } // e.g., Savings, Current, etc.
        public string? UPIId { get; set; }
        public bool? IsPrimaryAccount { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
    }

}
