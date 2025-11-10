using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Bank
{

    public class GetBankResponseDTO
    {
        public string? Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountType { get; set; } // e.g., Savings, Current, etc.
        public bool? IsPrimaryAccount { get; set; }
        public bool? IsInfoVerified { get; set; }
        public string? IFSCCode { get; set; }
        public string? BranchName { get; set; }
        public string? UPIId { get; set; }

        public bool? IsPrimary { get; set; }
        public bool? IsEditAllowed { get; set; }
        public bool IsActive { get; set; }
    }

}
