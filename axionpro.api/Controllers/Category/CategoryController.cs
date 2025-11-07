 
using axionpro.application.DTOs.Category;

//using axionpro.application.Features.AttendanceCmd.Command;
using axionpro.application.Features.CategoryCmd.Command;
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


        [HttpPost("get")]
        public async Task<IActionResult> GetAllMainCategories([FromBody] CategoryRequestDTO? categoryRequestDTO)
        {
            _logger.LogInfo("Received  request to get categories from userId: {LoginId}" );
            var command = new GetMainCategoryCommand(categoryRequestDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }


        [HttpPost("getalltendermaincategory")]
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


        [HttpPost("getallmainchildcategory")]
        public async Task<IActionResult> GetAllMainChildCategories([FromBody] CategoryRequestDTO? categoryRequestDTO)
        {
            _logger.LogInfo("Received  request to get sub-categories from userId: {LoginId}" );
            var command = new GetMainChildCategoryCommand(categoryRequestDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }


    }
}
