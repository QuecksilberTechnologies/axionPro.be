using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Gender;
using axionpro.application.DTOS.Location;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ILocationRepository
    {
        Task<ApiResponse<List<GetCountryOptionResponseDTO?>>> GetCountryOptionAsync(GetCountryOptionRequestDTO dto);
        Task<ApiResponse<List<GetDistrictOptionResponseDTO?>>> GetDistrictOptionAsync(GetDistrictOptionRequestDTO dto);
        Task<ApiResponse<List<GetStateOptionResponseDTO?>>> GetStateOptionAsync(GetStateOptionRequestDTO dto);

        Task<List<Country>> GetAllAsync();
        Task<Country> GetByIdAsync(int id);
        Task AddAsync(Country country);
        Task UpdateAsync(Country country);
        Task DeleteAsync(int id);
    }
}
