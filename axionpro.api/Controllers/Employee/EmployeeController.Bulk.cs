using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Employee;

public partial class EmployeeController
{
        #region Employee Invitations

        /// <summary>Sends pending or failed welcome invitations for a completed Employee import.</summary>
        /// <remarks>
        /// Angular integration pending. Requires Add or Import on EMP_LIST. Body: JobId, ModuleId, OperationId,
        /// optional RowNumbers (up to 100). This is an explicit email-sending action; import confirmation sends no emails.
        /// Sent rows are not resent. Sending or DeliveryUnknown rows require email-log review after interruption.
        /// Tokens are generated at dispatch time. Account creation is not repeated when invitations are retried.
        /// </remarks>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("bulk/send-invitations")]
        public async Task<IActionResult> SendBulkInvitations(
            [FromBody] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.SendInvitations), cancellationToken));
        }

        #endregion

        #region Employee Import Preview
        /// <summary>Validates Employee Excel/CSV or pasted data and saves a durable draft; master data is unchanged.</summary>
        /// <remarks>
        /// <para>Angular usage status: Not integrated yet.</para>
        /// <para>Send multipart/form-data with File OR PastedText, ModuleId, OperationId,
        /// optional SheetName and ColumnMappingJson (target field to source header).</para>
        /// <para>Uses the existing Employee permission pipeline and trusted login Tenant scope.</para>
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
                new PreviewEmployeeImportQuery(dto),
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
            return Ok(await _mediator.Send(new ManageEmployeeImportCommand(
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
            return Ok(await _mediator.Send(new ManageEmployeeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Get), cancellationToken));
        }

        /// <summary>Lists the caller's tenant-scoped imports, newest first; PageSize is limited to 100.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("bulk/jobs")]
        public async Task<IActionResult> ListBulkImports(
            [FromQuery] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.List), cancellationToken));
        }

        /// <summary>Retries failed or incomplete rows; already created records are never recreated.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("bulk/retry")]
        public async Task<IActionResult> RetryBulkImport(
            [FromBody] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Retry), cancellationToken));
        }

        /// <summary>Cancels pending work at a batch boundary. Previously committed records remain.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("bulk/cancel")]
        public async Task<IActionResult> CancelBulkImport(
            [FromBody] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ManageEmployeeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Cancel), cancellationToken));
        }

        /// <summary>Downloads the CSV column template. Reference values come from existing tenant master APIs.</summary>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("bulk/template")]
        public async Task<IActionResult> DownloadBulkTemplate(
            [FromQuery] axionpro.application.DTOS.Common.BulkImportJobRequestDTO dto,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ManageEmployeeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Template), cancellationToken);
            return File(System.Text.Encoding.UTF8.GetBytes((string)response.Data!),
                "text/csv; charset=utf-8", "Employee-template.csv");
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
            var response = await _mediator.Send(new ManageEmployeeImportCommand(
                dto, axionpro.application.Common.Enums.BulkImportAction.Get), cancellationToken);
            return File(axionpro.api.Common.BulkImportReport.Create(
                (axionpro.application.DTOS.Common.BulkImportJobResponseDTO)response.Data!),
                "text/csv; charset=utf-8", "Employee-import-" + jobId + ".csv");
        }

        #endregion


}

