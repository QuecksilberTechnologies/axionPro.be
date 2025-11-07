using axionpro.application.DTOs.Manager.ReportingType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IReportingTypeRepository
    {
        public Task<List<GetReportingTypeResponseDTO>> AddAsync(CreateReportingTypeRequestDTO dTO);
        public Task<List<GetReportingTypeResponseDTO>> AllAsync(GetReportingTypeRequestDTO dTO);
        public Task<GetReportingTypeResponseDTO?> GetByIdAsync(long id);
        public Task<bool> DeleteAsync(long id, long employeeId);
        public Task<bool> UpdateAsync(UpdateReportingTypeRequestDTO dto);
    }
}
