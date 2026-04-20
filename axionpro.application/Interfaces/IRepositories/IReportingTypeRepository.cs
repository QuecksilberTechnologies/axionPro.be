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
    public interface IReportingTypeRepository
    {
        public Task<GetReportingTypeResponseDTO> AddAsync(CreateReportingTypeRequestDTO dTO);
        public Task<PagedResponseDTO<GetReportingTypeResponseDTO>> AllAsync(GetReportingTypeRequestDTO dTO);
        public Task<GetReportingTypeResponseDTO?> GetByIdAsync(long id);
        public Task<bool> DeleteAsync(long id, long employeeId);
        public Task<bool> UpdateAsync(UpdateReportingTypeRequestDTO dto);
    }
}
