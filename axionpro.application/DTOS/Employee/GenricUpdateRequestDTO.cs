using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Employee
{
    /// <summary>
    /// post-request to update any employe info :edu/basic,personal,bank,exp 
    /// </summary>

    public class GenricUpdateRequestDTO
    {


        [Required]
        public string EncriptedId { get; set; } = null!; ///Record Id          
        public string EncriptedEmployeeId { get; set; } = null!; ///Record Id 
        public long EmployeeId { get; set; } /// EmployeeId
        public long Id { get; set; } /// EmployeeId
        [Required]
        public string EntityName { get; set; } = string.Empty;
        /// <summary>
        /// actual field name
        /// </summary>

        [Required]
        public string FieldName { get; set; } = string.Empty;
        /// <summary>
        ///  new value which is going to update!
        /// </summary>

        [Required]
        public object? FieldValue { get; set; }


    
    }
}
