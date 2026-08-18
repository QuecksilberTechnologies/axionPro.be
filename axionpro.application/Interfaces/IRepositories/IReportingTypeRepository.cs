// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the repository contract for  Reporting Type Repository.
// ================================================================

using axionpro.application.DTOs.Manager.ReportingType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOS.Pagination;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines the contract for ReportingTypeRepository.
    /// </summary>
    public interface IReportingTypeRepository
    {
        public Task<ReportingType?> AddAsync(ReportingType entity);
        public Task<PagedResponseDTO<GetReportingTypeResponseDTO>> AllAsync(long tenantId, GetReportingTypeRequestDTO dTO);
        public Task<GetReportingTypeResponseDTO?> GetByIdAsync(long id);
        public Task<bool> DeleteAsync(long id, long employeeId);
        public Task<bool> UpdateAsync(UpdateReportingTypeRequestDTO dto);
    }
}
