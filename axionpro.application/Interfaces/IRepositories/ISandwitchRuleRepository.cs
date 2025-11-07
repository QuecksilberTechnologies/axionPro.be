using axionpro.application.DTOs.SandwitchRule;
using axionpro.application.DTOs.SandwitchRule.DayCombination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ISandwitchRuleRepository
    {
        /// <summary>
        /// नई Day Combination जोड़ता है।
        /// </summary>
        /// <param name="request">Create DTO containing data to add</param>
        /// <returns>नया record object या उसका Id</returns>
        public Task<IEnumerable<GetDayCombinationResponseDTO>> AddDayCombinationAsync(CreateDayCombinationRequestDTO request);
        public Task<bool> UpdateDayCombinationAsync(UpdateDayCombinationRequestDTO request);
        public Task<bool> DeleteDayCombinationAsync(DeleteDayCombinationRequestDTO request);

        /// <summary>
        /// मौजूदा Day Combination अपडेट करता है।
        /// </summary>
        /// <param name="id">Combination Id</param>
        /// <param name="request">Update DTO containing updated data</param>
        /// <returns>Updated record object या null (अगर नहीं मिला)</returns>
      //  public Task<DayCombination?> UpdateDayCombinationAsync(int id, CreateDayCombinationRequestDTO request);

        /// <summary>
        /// Tenant के अनुसार सभी active combinations लाता है।
        /// </summary>
        public Task<IEnumerable<GetDayCombinationResponseDTO>> GetAllActiveDayCombinationsAsync(long tenantId, bool IsActive);
        Task<IEnumerable<GetLeaveSandwitchRuleResponseDTO>> GetAllActiveSandwichRulesAsync(long tenantId, bool isActive);
        public Task<IEnumerable<GetLeaveSandwitchRuleResponseDTO>> AddSandwichAsync(CreateLeaveSandwichRuleRequestDTO request);
        public Task<bool> UpdateSandwichAsync(UpdateLeaveSandwitchRuleRequestDTO request);
        public Task<bool> DeleteSandwichAsync(DeleteLeaveSandwitchRuleRequestDTO request);





    }
}
