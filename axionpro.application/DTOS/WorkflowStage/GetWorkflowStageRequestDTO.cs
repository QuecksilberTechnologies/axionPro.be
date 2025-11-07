using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.WorkflowStage
{
    public class GetWorkflowStageRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
      
        public bool? IsActive { get; set; }
     
    }
}
