using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
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
   
    public class CreateBankRequestDTO
    {
        /// <summary> TenantId Required</summary>

        [Required]
        public string UserEmployeeId { get; set; } = string.Empty;        
        public string EmployeeId { get; set; } = string.Empty;      
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? BranchName { get; set; }
        public string? AccountType { get; set; }
        public string? UPIId { get; set; }
        public bool IsPrimaryAccount { get; set; }= false;        
        public IFormFile? CancelledChequeFile { get; set; }






    }


}
