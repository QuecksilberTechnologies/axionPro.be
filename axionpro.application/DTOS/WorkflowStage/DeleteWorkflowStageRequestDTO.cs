using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.WorkflowStage
{
    public class DeleteWorkflowStageRequestDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public long RoleId { get; set; }
       

    }
}
