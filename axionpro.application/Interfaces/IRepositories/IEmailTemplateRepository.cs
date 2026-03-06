
using System;
using System.Collections.Generic;
using axionpro.domain.Entity;

using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
  public  interface  IEmailTemplateRepository
    {
       // Task<EmailTemplate?> GetTemplateByCodeAsync(string templateCode);
       // Task<List<EmailTemplate>> GetTemplateByCodeAsync(string TemplateCode);
        Task<EmailTemplate> GetTemplateByCodeAsync(string TemplateCode);

        Task<IEnumerable<EmailTemplate>> GetAllTemplatesAsync();
        Task AddTemplateAsync(EmailTemplate template);
        Task UpdateTemplateAsync(EmailTemplate template);
        Task DeleteTemplateAsync(int templateId);

    }
}
