using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave
{
    public class GetLeaveTypeRequestDTO: BaseRequest
    {

        public long EmployeeId { get; set; }
        public long TenantId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }     
        public int LeaveTypeId { get; set; }



    }
}
