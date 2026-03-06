using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.WorkflowStage
{
    public class GetWorkflowStageByIdRequestDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
      
       
     
    }
}
