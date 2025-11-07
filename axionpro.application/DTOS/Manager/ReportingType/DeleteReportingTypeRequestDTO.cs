using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Manager.ReportingType
{

    public class DeleteReportingTypeRequestDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public long RoleId { get; set; }
       

    }
}
