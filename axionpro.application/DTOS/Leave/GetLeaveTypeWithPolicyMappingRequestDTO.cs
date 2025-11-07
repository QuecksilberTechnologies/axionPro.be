using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave
{
    public class GetLeaveTypeWithPolicyMappingRequestDTO : BaseRequest
    {
        public long TenantId { get; set; }                  // Required
        public long EmployeeId { get; set; }                  // Required
        public int RoleId { get; set; }                  // Required
        public bool? IsActive { get; set; }              // Optional
                   
    }
}
