// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Email Template operations.
// ================================================================

using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Features.EmailTemplateCmd.Queries;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EmailTemplate
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly ILoggerService _logger;
        public EmailTemplateController(
            IMediator mediator,
            IEmailService emailService,
            ILoggerService logger)
        {
            _mediator = mediator;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet("get-template-by-code")]      
        public async Task<IActionResult> GetTemplateByCodeAsync([FromQuery] string code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new GetEmailTemplateByCodeQuery(code);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("send-template")]
        public async Task<IActionResult> SendTemplatedEmail([FromBody] SendEmailTemplatRequestDTO request)
        {
            _logger.LogInfo($"Sending email to {request.ToEmail} using template {request.TemplateCode}");

            var result = await _emailService.SendTemplatedEmailAsync(
                request.TemplateCode,
                request.ToEmail,
                request.TenantId,
                request.Placeholders
            );


            return Ok("Email sent successfully.");
        }
    }
}
