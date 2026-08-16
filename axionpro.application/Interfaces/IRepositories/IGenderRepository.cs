// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for gender data and option projections.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Gender;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines persistence operations for gender data and option projections.
    /// </summary>
    public interface IGenderRepository
    {
        /// <summary>
        /// Gets available gender options as query data.
        /// </summary>
        /// <param name="dto">The option query criteria.</param>
        /// <returns>The available gender option projections.</returns>
        Task<List<GetGenderOptionResponseDTO>> GetOptionAsync(GetOptionRequestDTO dto);

        Task<Gender> GetByIdAsync(long id);
        Task<IEnumerable<Gender>> GetAllAsync();
        Task AddAsync(Gender gender);
        Task UpdateAsync(Gender gender);
        Task DeleteAsync(long id);
    }
}
