
using axionpro.application.DTOs.WorkflowStage;
 
 
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IWorkflowStagesRepository
    {

        public Task<List<GetWorkflowStageResponseDTO>> AddAsync(CreateWorkflowStageRequestDTO dTO);
        public Task<List<GetWorkflowStageResponseDTO>> AllAsync(GetWorkflowStageRequestDTO dTO);      
        public Task<GetWorkflowStageResponseDTO?> GetByIdAsync(long id);
        public Task<bool> DeleteAsync(long id, long employeeId);
        public Task<bool> UpdateAsync(UpdateWorkflowStageRequestDTO dto);
    }
}
