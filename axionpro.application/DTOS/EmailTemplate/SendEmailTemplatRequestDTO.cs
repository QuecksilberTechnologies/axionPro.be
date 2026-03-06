using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.EmailTemplate
{
    public class SendEmailTemplatRequestDTO
    {
        public string TemplateCode { get; set; }
        public string ToEmail { get; set; }
        public long? TenantId { get; set; }
        public Dictionary<string, string> Placeholders { get; set; }
    }
}
