// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Option operations.
// ================================================================

using axionpro.application.DTOs.Operation;
 
using axionpro.application.Features.OperationCmd.Commands;
using axionpro.application.Features.OperationCmd.Queries;
 
using axionpro.application.Features.TransportCmd.Commands;
using axionpro.application.Features.TransportCmd.Queries;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Operation
{
    /// <summary>
    /// handled-operation-related-actions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public OptionController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Used-In-Angular: retrieves options.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): OptionApi.getOptions (app/core/services/option-api.ts:31).</para>
        /// <para>Angular purpose: retrieves options.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
        /// </remarks>
        [HttpGet("get")]        
        public async Task<IActionResult> GetAllOperationAsyc([FromQuery] GetOperationRequestDTO operationRequestDTO)
        {
            _logger.LogInfo($"Received request to get operationRequestDTO from userId: {operationRequestDTO.EmployeeId}");

            var command = new GetAllOperationCommand(operationRequestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: creates option.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): OptionApi.addOption (app/core/services/option-api.ts:37).</para>
        /// <para>Angular purpose: creates option.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
        /// </remarks>
        [HttpPost("create")]
        
        public async Task<IActionResult> CreateOperation([FromBody] CreateOperationRequestDTO createOperationDTO)
        {
           
            _logger.LogInfo($"Received request to create a new operationRequestDTO: {createOperationDTO.OperationName}");

            var command = new CreateOperationCommand(createOperationDTO);
            var result = await _mediator.Send(command);

          
            return Ok(result);
        }

        /// <summary>
        /// Used-In-Angular: updates option.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): OptionApi.updateOption (app/core/services/option-api.ts:43).</para>
        /// <para>Angular purpose: updates option.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
        /// </remarks>
        [HttpPost("update")]
               
        public async Task<IActionResult> UpdateOperation([FromBody] UpdateOperationRequestDTO updateOperationDTO)
        {
            _logger.LogInfo("Received request for update a leave" + updateOperationDTO.ToString());
            var command = new UpdateOperationCommand(updateOperationDTO);
            var result = await _mediator.Send(command);            
            return Ok(result);
        }
        /// <summary>
        /// Used-In-Angular: updates option.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): OptionApi.updateOption (app/core/services/option-api.ts:50).</para>
        /// <para>Angular purpose: updates option.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
        /// </remarks>
         [Authorize]
        [HttpGet("has-access")]
        public async Task<IActionResult> HasPageOperationAccess([FromQuery] GetCheckOperationPermissionRequestDTO? checkOperationPermissionRequest)
        {
            

            var query = new GetPageOperationPermissionQuery(checkOperationPermissionRequest);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        


        //  [HttpPost("getalltendermaincategory")]
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


        //[HttpPost("getallmainchildcategory")]
        //public async Task<IActionResult> GetAllMainChildCategories([FromBody] CategoryRequestDTO? categoryRequestDTO)
        //{
        //    _logger.LogInfo("Received  request to get sub-categories from userId: {LoginId}" + categoryRequestDTO.Id.ToString());
        //    var command = new GetMainChildCategoryCommand(categoryRequestDTO);
        //    var result = await _mediator.Send(command);
        //    if (!result.IsSuccecced)
        //    {
        //        return Unauthorized(result);
        //    }
        //    return Ok(result);
        //}


    }



}
