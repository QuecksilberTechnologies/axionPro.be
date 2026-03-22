using axionpro.application.DTOs.Entity;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Entity
{

    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public EntityController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpGet("get")]
        public IActionResult GetStaticEntityNames([FromQuery] GetEntityNameRequestDTO dTO)
        {
            var entities = new List<GetEntityNameResponseDTO>
    {
        new GetEntityNameResponseDTO { Id = 1, Name = "Employee" },
        new GetEntityNameResponseDTO { Id = 2, Name = "EmployeeBankDetail" },
        new GetEntityNameResponseDTO { Id = 3, Name = "EmployeeExperience" },
        new GetEntityNameResponseDTO { Id = 4, Name = "EmployeeFamily" },
        new GetEntityNameResponseDTO { Id = 5, Name = "EmployeePersonalDetail" },
        new GetEntityNameResponseDTO { Id = 6, Name = "EmployeeEducation" },
        new GetEntityNameResponseDTO { Id = 6, Name = "EmployeeDependent" },
        new GetEntityNameResponseDTO { Id = 6, Name = "EmployeeInsurance" },
        new GetEntityNameResponseDTO { Id = 6, Name = "EmployeeContact" }
    };

            return Ok(ApiResponse<List<GetEntityNameResponseDTO>>.Success(entities));
        }

    }
}