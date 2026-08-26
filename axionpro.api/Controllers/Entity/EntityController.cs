// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Entity operations.
// ================================================================

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
        /// <summary>
        /// Get Static Entity Names.
        /// </summary>
        /// <remarks>
        /// Handles the request to get static entity names.
        /// </remarks>
        /// <param name="dTO">The query parameters used to get static entity names.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
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
