using axionpro.application.DTOs.MenuDataView;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.MenuStructureView
{
    /// <summary>
    /// Menu-data structure view.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]


    public class MenuStructureController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration


        public MenuStructureController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;  // Logger service ko inject karna
        }

        /// <summary>
        /// Get all employees that belong to the specified tenant.
        /// </summary>
        [HttpPost("get-menus-structure")]

        public async Task<IActionResult> GetAllMenuStructure([FromBody] GetEmployeeTypeRequestDTO requestDto)
        {
            try
            {
                // Dummy data list
                var menuDisplay = new List<GetMenuDataStructureResponseDTO>
                 {
            new GetMenuDataStructureResponseDTO
            {
                Id = 1,
                DisplayOn = "Left-Menu"
                
            },
              new GetMenuDataStructureResponseDTO
            {
                Id = 1,
                DisplayOn = "Top-Bar"

            }

            };

                // Wrap in ApiResponse
                var response = new ApiResponse<List<GetMenuDataStructureResponseDTO>>(
                        menuDisplay,
                     "Menu display structure fetched successfully.",
                              true
  );


                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<List<GetMenuDataStructureResponseDTO>>(
                    null!,
                    "Failed to fetch Menu display structure.",
                    false
                );
                errorResponse.Errors.Add(ex.Message);
                return StatusCode(500, errorResponse);
            }
        }



    }
}