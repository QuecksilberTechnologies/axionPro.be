// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Category operations.
// ================================================================


using axionpro.application.DTOs.Category;


//using axionpro.application.Features.AttendanceCmd.Command;
using axionpro.application.Features.CategoryCmd.Command;
using axionpro.application.Features.CategoryCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Category
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public CategoryController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: POST /api/category/get.</para>
        //         /// </remarks>
        //         [HttpPost("get")]
        //
        //         public async Task<IActionResult> GetAllMainCategories([FromBody] CategoryRequestDTO? categoryRequestDTO)
        //         {
        //             _logger.LogInfo("Received  request to get categories from userId: {LoginId}" );
        //             var command = new GetMainCategoryCommand(categoryRequestDTO);
        //             var result = await _mediator.Send(command);
        //             if (!result.IsSucceeded)
        //             {
        //                 return Unauthorized(result);
        //             }
        //             return Ok(result);
        //         }
        #endregion


     
        //public async Task<IActionResult> GetAllTenderMainCategories([FromBody] TenderCategoryRequestDTO? tenderCategoryRequestDTO)
        //{
        //    _logger.LogInfo("Received  request to get categories from userId: {LoginId}" + tenderCategoryRequestDTO.Id.ToString());
        //    var command = new GetTenderMainCategoryRequestCommand(tenderCategoryRequestDTO);
        //    var result = await _mediator.Send(command);
        //    if (!result.IsSuccecced)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: POST /api/category/getallmainchildcategory.</para>
        //         /// </remarks>
        //
        //
        //         [HttpPost("getallmainchildcategory")]
        //         public async Task<IActionResult> GetAllMainChildCategories([FromBody] CategoryRequestDTO? categoryRequestDTO)
        //         {
        //             _logger.LogInfo("Received  request to get sub-categories from userId: {LoginId}" );
        //             var command = new GetMainChildCategoryCommand(categoryRequestDTO);
        //             var result = await _mediator.Send(command);
        //             if (!result.IsSucceeded)
        //             {
        //                 return Unauthorized(result);
        //             }
        //             return Ok(result);
        //         }
        #endregion


    }
}
