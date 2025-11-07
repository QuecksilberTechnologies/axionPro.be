using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Gender
{
    public class GetGenderRequestDTO
    {
   
        public long? TenantId { get; set; }                  // Required
        public long? EmployeeId { get; set; }                  // Required
        public int? RoleId { get; set; }                   
    }
}
