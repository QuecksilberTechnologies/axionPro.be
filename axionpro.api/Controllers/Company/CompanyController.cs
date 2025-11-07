using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.Verify;
 
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.application.Features.VerifyEmailCmd.Commands;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace axionpro.api.Controllers.Company;

[Route("api/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoggerService _logger;  // Logger service ka declaration

    public CompanyController(IMediator mediator, ILoggerService logger)
    {
        _mediator = mediator;
        _logger = logger;  // Logger service ko inject karna
    }
   

    [HttpGet("{firstname}/{lastname}")]
   // [Authorize]
    public async Task<IActionResult> Get(string firstname, string lastname)
    {
        _logger.LogInfo("Company is created");
        return Ok();
    }

}
