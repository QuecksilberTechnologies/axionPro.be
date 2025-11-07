using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class TenantIndustryRepository: ITenantIndustryRepository
    {
        private readonly WorkforceDbContext _context;
        private ILogger<TenantIndustryRepository> _logger;
        public TenantIndustryRepository(WorkforceDbContext context, ILogger<TenantIndustryRepository> logger)
        {
            this._context = context;
            this._logger = logger;
        }
        public async Task<List<TenantIndustry>> GetAllActiveIndustriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all active tenant industries.");
                return await _context.TenantIndustries
                                     .Where(i => i.IsActive)
                                     .OrderBy(i => i.IndustryName)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching industries.");
                return new List<TenantIndustry>();
            }
        }

        public async Task<TenantIndustry?> GetIndustryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching industry by ID: {Id}", id);
                return await _context.TenantIndustries.FirstOrDefaultAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching industry by ID: {Id}", id);
                return null;
            }
        }


        public async Task<TenantIndustry?> GetAllIndustryByAsync()
        {
            try
            {
              
                return await _context.TenantIndustries.FirstOrDefaultAsync(i => i.IsActive == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching industry");
                return null;
            }
        }

        public async Task AddIndustryAsync(TenantIndustry industry)
        {
            try
            {
                _logger.LogInformation("Adding new industry: {Name}", industry.IndustryName);
                await _context.TenantIndustries.AddAsync(industry);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding industry: {Name}", industry.IndustryName);
            }
        }

        public async Task UpdateIndustryAsync(TenantIndustry industry)
        {
            try
            {
                _logger.LogInformation("Updating industry ID: {Id}", industry.Id);
                _context.TenantIndustries.Update(industry);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while updating industry ID: {Id}", industry.Id);
            }
        }

        public async Task<bool> IsIndustryExistsAsync(int id)
        {
            try
            {
                _logger.LogInformation("Checking if industry exists: {Id}", id);
                return await _context.TenantIndustries.AnyAsync(i => i.Id == id && i.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error checking existence for industry ID: {Id}", id);
                return false;
            }
        }

    }
}
