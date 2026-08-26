// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes employee-type endpoints and delegates application operations to handlers.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.EmployeeType;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.EmployeeType;
using axionpro.application.Features.EmployeeTypeCmd.Handlers;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EmployeeType
{
    /// <summary>
    /// Exposes employee-type endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTypeController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeTypeController> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeController"/> class.
        /// </summary>
        public EmployeeTypeController(
            IMediator mediator,
            ILogger<EmployeeTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #endregion

        #region Queries

        /// <summary>
        /// Get All Employee Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all employee type.
        /// </remarks>
        /// <param name="requestDto">The query parameters used to get all employee type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("get")]
        public IActionResult GetAllEmployeeType(
            [FromQuery] application.DTOS.Employee.Type.GetEmployeeTypeRequestDTO requestDto)
        {
            _logger.LogInformation("Fetching all employee types.");

            var employeeTypes = new List<GetEmployeeTypeResponseDTO>
            {
                new()
                {
                    Id = 1,
                    TypeName = "Full-Time",
                    Description = "Permanent employee with all benefits",
                    IsActive = true
                },
                new()
                {
                    Id = 2,
                    TypeName = "Contract",
                    Description = "Contract-based employee",
                    IsActive = true
                },
                new()
                {
                    Id = 3,
                    TypeName = "Intern",
                    Description = "Internship employee",
                    IsActive = true
                },
                new()
                {
                    Id = 4,
                    TypeName = "Freelancer",
                    Description = "External resource",
                    IsActive = false
                }
            };

            return Ok(ApiResponse<List<GetEmployeeTypeResponseDTO>>.Success(
                employeeTypes,
                AppConstants.SuccessMessages.EmployeeTypesRetrieved));
        }

        /// <summary>
        /// Get All Employee Type.
        /// </summary>
        /// <remarks>
        /// Handles the request to get all employee type.
        /// </remarks>
        /// <param name="requestDTO">The query parameters used to get all employee type.</param>
        /// <returns>An HTTP response containing the result of the operation.</returns>
        [HttpGet("option")]
        public async Task<IActionResult> GetAllEmployeeType([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInformation("Fetching employee-type options.");

            var result = await _mediator.Send(new GetEmployeeTypeOptionQuery(requestDTO));
            return Ok(result);
        }

        #endregion
    }
}
