using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Gender;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface  IGenderRepository
    {
        Task<ApiResponse<List<GetGenderOptionResponseDTO?>>> GetOptionAsync(GetOptionRequestDTO dto);

        Task<Gender> GetByIdAsync(long id);
        Task<IEnumerable<Gender>> GetAllAsync();
        Task AddAsync(Gender gender);
        Task UpdateAsync(Gender gender);
        Task DeleteAsync(long id);
    }
}
