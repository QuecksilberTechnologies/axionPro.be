using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.Bank
{
    /// <summary>
    /// Employee create request
    /// </summary>
    /// 
    /// <summary> TenantId Required</summary>
   
    public class CreateBankRequestDTO:BaseRequest
    {
        /// <summary> TenantId Required</summary>

        [Required]
      
        public string EmployeeId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }        
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? BranchName { get; set; }
        public string? AccountType { get; set; }
        public string? UPIId { get; set; }
        public bool IsPrimaryAccount { get; set; }= false;        
        public string? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public bool IsEditAllowed { get; set; } = true;
        public bool IsInfoVerified { get; set; } = false;


    }


}
