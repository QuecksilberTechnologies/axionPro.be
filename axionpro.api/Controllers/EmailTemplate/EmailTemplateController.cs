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
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves email template by code.</para>
                /// <para>Handler flow: GetEmailTemplateByCodeQuery is processed by GetEmailTemplateByCodeQueryHandler; operation(s): GetTemplateByCodeAsync.</para>
                /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); EmailTemplateDTO: TemplateCode (string), Subject (string), Body (string), FromEmail (string), FromName (string)</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: GET /api/emailtemplate/get-template-by-code.</para>
                /// </remarks>

                [HttpGet("get-template-by-code")]
                public async Task<IActionResult> GetTemplateByCodeAsync([FromQuery] string code)
                {
                    _logger.LogInfo($"Getting email templates for code: {code}");

                    var query = new GetEmailTemplateByCodeQuery(code);
                    var result = await _mediator.Send(query);

                    return Ok(result);
                }
                /// <summary>
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: performs the Angular function send templated email.</para>
                /// <para>Handler flow: No application request/handler class was statically resolved from the controller action.</para>
                /// <para>Response DTO property analysis: No concrete response DTO properties were statically resolved from the request/handler declaration.</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/emailtemplate/send-template.</para>
                /// </remarks>

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
