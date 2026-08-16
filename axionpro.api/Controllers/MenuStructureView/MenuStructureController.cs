// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes the menu-display structure endpoint.
// ================================================================

using axionpro.application.DTOs.MenuDataView;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.MenuStructureView
{
    /// <summary>
    /// Exposes menu-data structure endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MenuStructureController : ControllerBase
    {
        /// <summary>
        /// Gets the available menu-display structure.
        /// </summary>
        [HttpPost("get-menus-structure")]
        public IActionResult GetAllMenuStructure([FromBody] GetEmployeeTypeRequestDTO requestDto)
        {
            var menuDisplay = new List<GetMenuDataStructureResponseDTO>
            {
                new()
                {
                    Id = 1,
                    DisplayOn = "Left-Menu"
                },
                new()
                {
                    Id = 1,
                    DisplayOn = "Top-Bar"
                }
            };

            return Ok(ApiResponse<List<GetMenuDataStructureResponseDTO>>.Success(
                menuDisplay,
                AppConstants.SuccessMessages.MenuDisplayStructureRetrieved));
        }
    }
}
