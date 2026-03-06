using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.EmailTemplateCmd.Queries
{
    public class GetEmailTemplateByCodeQuery : IRequest<ApiResponse<EmailTemplateDTO>>
    {
        public string Code { get; set; }

        public GetEmailTemplateByCodeQuery(string code)
        {
            Code = code;
        }
    }
}
