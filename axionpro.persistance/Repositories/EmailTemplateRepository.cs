using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly ILogger<EmailTemplateRepository> _logger;

        public EmailTemplateRepository(WorkforceDbContext context, ILogger<EmailTemplateRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EmailTemplate?> GetTemplateByCodeAsync(string templateCode)
        {
            try
            {
                return await _context.EmailTemplates
                    .Where(t => t.TemplateCode == templateCode && t.IsActive)
                    .OrderByDescending(t => t.AddedDateTime)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching template by code: {TemplateCode}", templateCode);
                return null;
            }
        }

        public Task AddTemplateAsync(EmailTemplate template)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTemplateAsync(int templateId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EmailTemplate>> GetAllTemplatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateTemplateAsync(EmailTemplate template)
        {
            throw new NotImplementedException();
        }
    }
}
