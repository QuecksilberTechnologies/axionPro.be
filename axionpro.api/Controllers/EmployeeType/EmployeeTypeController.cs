// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes department endpoints and delegates application errors to middleware.
// ================================================================

using axionpro.application.DTOS.Employee.Type;

using axionpro.application.Features.EmployeeTypeCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EmployeeType
{
    [ApiController]
    [Route("api/[controller]")]

    public class EmployeeTypeController : ControllerBase    {
        #region Bulk Import Preview

        /// <summary>Validates EmployeeType Excel/CSV or pasted data and saves a durable draft; master data is unchanged.</summary>
        /// <remarks>
        /// <para>Angular usage status: Not integrated yet.</para>
        /// <para>Send multipart/form-data with File OR PastedText, ModuleId, OperationId,
        /// optional SheetName and ColumnMappingJson (target field to source header).</para>
        /// <para>Uses the existing EmployeeType permission pipeline and trusted login Tenant scope.</para>
        /// <para>Returns JobId, source row numbers, mappings, matches and errors.
        /// Confirm only when CanCommit is true. RequestId supports safe retries of the same upload.</para>
        /// </remarks>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("bulk/preview")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(axionpro.application.Constants.BulkImportConstants.MaxFileBytes + 65536)]
        public async Task<IActionResult> PreviewBulkImport(
            [FromForm] axionpro.application.DTOS.Common.BulkImportPreviewRequestDTO dto,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new PreviewEmployeeTypeImportQuery(dto),
                cancellationToken);
            return Ok(result);
        }

        #endregion


        #region Durable Bulk Import

        /// <summary>Confirms the saved preview and queues it now or at ScheduledAtUtc. Repeated confirmation is idempotent.</summary>
        /// <remarks>Requires Add or Import operation on this module. No replacement row data is accepted.</remarks>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("bulk/confirm")]
        public async Task<IActionResult> ConfirmBulkImport(
            [FromBody] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeTypeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Confirm), cancellationToken));
        }

        /// <summary>Gets the caller's saved import including source rows, progress and errors.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("bulk/jobs/{jobId:guid}")]
        public async Task<IActionResult> GetBulkImport(
            Guid jobId,
            [FromQuery] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            dto.JobId = jobId;
            return Ok(await _mediator.Send(new ManageEmployeeTypeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Get), cancellationToken));
        }

        /// <summary>Lists the caller's tenant-scoped imports, newest first; PageSize is limited to 100.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("bulk/jobs")]
        public async Task<IActionResult> ListBulkImports(
            [FromQuery] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeTypeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.List), cancellationToken));
        }

        /// <summary>Retries failed or incomplete rows; already created records are never recreated.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("bulk/retry")]
        public async Task<IActionResult> RetryBulkImport(
            [FromBody] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeTypeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Retry), cancellationToken));
        }

        /// <summary>Cancels pending work at a batch boundary. Previously committed records remain.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("bulk/cancel")]
        public async Task<IActionResult> CancelBulkImport(
            [FromBody] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeTypeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Cancel), cancellationToken));
        }

        /// <summary>Downloads the CSV column template. Reference values come from existing tenant master APIs.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("bulk/template")]
        public async Task<IActionResult> DownloadBulkTemplate(
            [FromQuery] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ManageEmployeeTypeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Template), cancellationToken);
            return File(System.Text.Encoding.UTF8.GetBytes((string)response.Data!),
                "text/csv; charset=utf-8", "EmployeeType-template.csv");
        }

        /// <summary>Downloads source row numbers, results and corrections for the caller's import.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("bulk/jobs/{jobId:guid}/report")]
        public async Task<IActionResult> DownloadBulkReport(
            Guid jobId,
            [FromQuery] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            dto.JobId = jobId;
            var response = await _mediator.Send(new ManageEmployeeTypeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Get), cancellationToken);
            return File(axionpro.api.Common.BulkImportReport.Create(
                (axionpro.application.DTOS.Common.BulkImportJobResponseDTO)response.Data!),
                "text/csv; charset=utf-8", "EmployeeType-import-" + jobId + ".csv");
        }

        #endregion

        private readonly IMediator _mediator;
        public EmployeeTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Tenant master operations
        /// <summary>Creates one tenant-owned type. Requires Add on TENANT_EMPLOYEE_TYPES.</summary>
        /// <remarks>Angular integration pending. Body: TypeName, Description, Remark, IsActive, ModuleId, OperationId.
        /// Returns ApiResponse of the created type; ownership/audit IDs are server assigned.</remarks>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeTypeDTO dto, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new CreateEmployeeTypeCommand(dto), cancellationToken));
        }

        /// <summary>Returns paged database types for the authenticated tenant, including inactive types.</summary>
        /// <remarks>Replaces the old hard-coded list. Query: ModuleId, OperationId, PageNumber=1, PageSize=20.</remarks>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] GetEmployeeTypeRequestDTO dto, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetEmployeeTypesQuery(dto), cancellationToken));
        }

        /// <summary>Returns active, non-deleted EmployeeType options for this tenant only.</summary>
        /// <remarks>Query now requires ModuleId and OperationId from the existing menu/permission context.</remarks>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("option")]
        public async Task<IActionResult> Options([FromQuery] GetEmployeeTypeRequestDTO dto, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetEmployeeTypeOptionQuery(dto), cancellationToken));
        }
        #endregion
    }
}
