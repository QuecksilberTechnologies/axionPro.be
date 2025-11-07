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
        /// Get all operations.
        /// </summary>
        [HttpGet("get-operations")]
        public async Task<IActionResult> GetAllOperationAsyc([FromQuery] GetOperationRequestDTO operationRequestDTO)
        {
            _logger.LogInfo($"Received request to get operationRequestDTO from userId: {operationRequestDTO.EmployeeId}");

            var command = new GetAllOperationCommand(operationRequestDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }



        /// <summary>
        /// Get insert operation.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOperation([FromBody] CreateOperationRequestDTO createOperationDTO)
        {
            if (createOperationDTO == null)
            {
                _logger.LogInfo("Received null request for creating operationRequestDTO .");  // ✅ अब सही है
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            _logger.LogInfo($"Received request to create a new operationRequestDTO: {createOperationDTO.OperationName}");

            var command = new CreateOperationCommand(createOperationDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update Operation.
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> UpdateOperation([FromBody] UpdateOperationRequestDTO updateOperationDTO)
        {
            _logger.LogInfo("Received request for update a leave" + updateOperationDTO.ToString());
            var command = new UpdateOperationCommand(updateOperationDTO);
            var result = await _mediator.Send(command);
            if (!result.IsSucceeded)
            {
                return Ok(result);
            }
            return Ok(result);
        }
         [Authorize]
        [HttpGet("has-page-operation-access")]
        public async Task<IActionResult> HasPageOperationAccess([FromQuery] GetCheckOperationPermissionRequestDTO? checkOperationPermissionRequest)
        {
            if (checkOperationPermissionRequest == null)
            {
              //  _logger.LogWarning("❌ Received null request for checking operation permission.");
               // return BadRequest(new ApiResponse<HasAccessOperationDTO>(false, "Invalid request", null));
            }

        //    _logger.LogInformation("📢 Checking operation permission for RoleId: {RoleId}, ModuleId: {ModuleId}, OperationId: {OperationId}",
             //   checkOperationPermissionRequest.RoleId, checkOperationPermissionRequest.ProjectChildModuleDetailId, checkOperationPermissionRequest.OperationId);

            var query = new GetPageOperationPermissionQuery(checkOperationPermissionRequest);
            var result = await _mediator.Send(query);

            if (!result.IsSucceeded)
            {
            //    _logger.LogWarning("⛔ Permission denied for RoleId: {RoleId}, ModuleId: {ModuleId}, OperationId: {OperationId}",
                   // checkOperationPermissionRequest.RoleId, checkOperationPermissionRequest.ProjectChildModuleDetailId, checkOperationPermissionRequest.OperationId);
                return Unauthorized(result);
            }

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
