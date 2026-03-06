using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.EmailTemplate
{
    public class EmailTemplateDTO
    {
        public string TemplateCode { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
 