using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.WorkflowStage
{
    public class UpdateWorkflowStageRequestDTO
    {
        public long Id { get; set; }             // Existing TicketType Id
        public long EmployeeId { get; set; }     // Id of the employee making the update
        public int RoleId { get; set; }          // Role of the employee
    
        public string? ColorKey { get; set; }

        public string? StageName { get; set; }

        public int? StageOrder { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        



    }
}
