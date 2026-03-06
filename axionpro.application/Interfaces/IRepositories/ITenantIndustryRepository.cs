
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ITenantIndustryRepository
    {
        Task<List<TenantIndustry>> GetAllActiveIndustriesAsync();
        Task<TenantIndustry?> GetIndustryByIdAsync(int id);
        Task<TenantIndustry?> GetAllIndustryByAsync();
        Task AddIndustryAsync(TenantIndustry industry);
        Task UpdateIndustryAsync(TenantIndustry industry);
        Task<bool> IsIndustryExistsAsync(int id);

    }
}
