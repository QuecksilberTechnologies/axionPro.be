using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.Features.EmailTemplateCmd.Queries;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.ILogger;
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

        /// <summary>
        /// Get email templates by template code
        /// </summary>
        /// <param name="code">Template code</param>
        /// <returns>List of matching email templates</returns>
        [HttpGet("get-template-by-code")]
        public async Task<IActionResult> GetTemplateByCodeAsync([FromQuery] string code)
        {
            _logger.LogInfo($"Getting email templates for code: {code}");

            var query = new GetEmailTemplateByCodeQuery(code);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
                _logger.LogInfo($"No templates found for code: {code}");
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Send email using template
        /// </summary>
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

            if (!result)
            {
                _logger.LogError($"Failed to send email to {request.ToEmail} using template {request.TemplateCode}");
                return StatusCode(500, "Failed to send email.");
            }

            return Ok("Email sent successfully.");
        }
    }
}
