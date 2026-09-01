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
                /// Not-Used-In-Angular.
                /// </summary>
                /// <remarks>
                /// <para>Angular usage status: Not-Used-In-Angular.</para>
                /// <para>API endpoint purpose: retrieves all menu structure.</para>
                /// <para>Handler flow: No application request/handler class was statically resolved from the controller action.</para>
                /// <para>Response DTO property analysis: No concrete response DTO properties were statically resolved from the request/handler declaration.</para>
                /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
                /// <para>Backend endpoint: POST /api/menustructure/get-menus-structure.</para>
                /// </remarks>
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
