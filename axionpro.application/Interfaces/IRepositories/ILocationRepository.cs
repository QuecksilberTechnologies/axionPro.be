// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for location data and option projections.
// ================================================================

using axionpro.application.DTOS.Location;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines persistence operations for location data and option projections.
    /// </summary>
    public interface ILocationRepository
    {
        /// <summary>
        /// Gets active country option projections.
        /// </summary>
        Task<List<GetCountryOptionResponseDTO>> GetCountryOptionAsync(GetCountryOptionRequestDTO dto);

        /// <summary>
        /// Gets active state option projections for a country.
        /// </summary>
        Task<List<GetStateOptionResponseDTO>> GetStateOptionAsync(GetStateOptionRequestDTO dto);

        /// <summary>
        /// Gets active district option projections for a state.
        /// </summary>
        Task<List<GetDistrictOptionResponseDTO>> GetDistrictOptionAsync(GetDistrictOptionRequestDTO dto);

        /// <summary>
        /// Determines whether an active state exists for the supplied identifier.
        /// </summary>
        Task<bool> IsActiveStateAsync(int stateId);

        Task<List<Country>> GetAllAsync();
        Task<Country> GetByIdAsync(int id);
        Task AddAsync(Country country);
        Task UpdateAsync(Country country);
        Task DeleteAsync(int id);
    }
}
