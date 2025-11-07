using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.WorkflowStage
{
    public class CreateWorkflowStageRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }      

        public string StageName { get; set; } = null!;
        public string? ColorKey { get; set; }        

        public int StageOrder { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }


    }
}
