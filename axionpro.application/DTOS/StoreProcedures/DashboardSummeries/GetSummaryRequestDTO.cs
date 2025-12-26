using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.StoreProcedures.DashboardSummeries
{
    public class GetSummaryRequestDTO : BaseRequest
    {
        public long TenantId { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }


    }
}
