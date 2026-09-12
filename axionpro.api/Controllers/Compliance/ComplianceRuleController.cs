using axionpro.application.DTOS.Compliances.ComplianceRule;
using axionpro.application.Features.ComplianceCmd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Compliance
{
    [Authorize]
    [Route("api/[controller]")]
    public class ComplianceRuleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ComplianceRuleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateComplianceRuleRequestDTO request)
        {
            return Ok(await _mediator.Send(new CreateComplianceRuleCommand(request)));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateComplianceRuleRequestDTO request)
        {
            return Ok(await _mediator.Send(new UpdateComplianceRuleCommand { DTO = request }));
        }
    }
}
