using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.Bank
{
    /// <summary>
    /// Update Bank request
    /// </summary>
    /// 
    /// <summary> TenantId Required</summary>

    public class UpdateBankReqestDTO
    {
        /// <summary> UserEmployeeId Required</summary>

        [Required]
        public string UserEmployeeId { get; set; } = string.Empty;
        [Required]
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? BranchName { get; set; }
        public string? AccountType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? UPIId { get; set; }
        public bool IsPrimaryAccount { get; set; }= false;        
        public IFormFile? CancelledChequeFile { get; set; }


        public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();






    }


}
