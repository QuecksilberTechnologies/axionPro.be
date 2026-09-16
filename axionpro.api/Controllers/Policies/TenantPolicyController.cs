using System.Text;
using axionpro.api.Common;
using axionpro.application.Common.Enums;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Policy;
using axionpro.application.Exceptions;
using axionpro.application.Features.GenericPolicyCmd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Policies;

/// <summary>Exposes tenant-isolated generic policy configuration and lifecycle operations.</summary>
[ApiController]
[Authorize]
[Route("api/TenantPolicy")]
public sealed class TenantPolicyController(IMediator mediator) : ControllerBase
{
    [HttpGet("lookups")]
    public async Task<IActionResult> GetLookups([FromQuery] PolicyAccessRequestDTO dto) => Ok(await mediator.Send(new GetPolicyLookupsQuery(dto)));

    [HttpGet("types")]
    public async Task<IActionResult> GetTypes([FromQuery] PolicyAccessRequestDTO dto) => Ok(await mediator.Send(new GetPolicyTypesQuery(dto)));

    [HttpPost("types")]
    public async Task<IActionResult> CreateType([FromBody] CreateGenericPolicyTypeRequestDTO dto) => Ok(await mediator.Send(new CreatePolicyTypeCommand(dto)));

    [HttpPut("types/{id:int}")]
    public async Task<IActionResult> UpdateType(int id, [FromBody] UpdateGenericPolicyTypeRequestDTO dto)
    {
        dto.Id = id;
        return Ok(await mediator.Send(new UpdateGenericPolicyTypeCommand(dto)));
    }

    [HttpPatch("types/{id:int}/status")]
    public async Task<IActionResult> ChangeTypeStatus(int id, [FromBody] ChangePolicyTypeStatusRequestDTO dto)
    {
        dto.Id = id;
        return Ok(await mediator.Send(new ChangePolicyTypeStatusCommand(dto)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PolicyListRequestDTO dto) => Ok(await mediator.Send(new GetPoliciesQuery(dto)));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] PolicyByIdRequestDTO dto)
    {
        dto.Id = id;
        return Ok(await mediator.Send(new GetPolicyQuery(dto)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePolicyRequestDTO dto) => Ok(await mediator.Send(new CreatePolicyCommand(dto)));

    [HttpPut("{policyId:long}/versions/{versionId:long}")]
    public async Task<IActionResult> UpdateDraft(long policyId, long versionId, [FromBody] UpdatePolicyDraftRequestDTO dto)
    {
        dto.PolicyId = policyId;
        dto.PolicyVersionId = versionId;
        return Ok(await mediator.Send(new UpdatePolicyDraftCommand(dto)));
    }

    [HttpPost("{policyId:long}/versions/clone")]
    public async Task<IActionResult> Clone(long policyId, [FromBody] ClonePolicyVersionRequestDTO dto)
    {
        dto.PolicyId = policyId;
        return Ok(await mediator.Send(new ClonePolicyVersionCommand(dto)));
    }

    [HttpPost("versions/{versionId:long}/transition")]
    public async Task<IActionResult> Transition(long versionId, [FromBody] PolicyTransitionRequestDTO dto)
    {
        dto.PolicyVersionId = versionId;
        return Ok(await mediator.Send(new TransitionPolicyCommand(dto)));
    }

    [HttpGet("resolve")]
    public async Task<IActionResult> Resolve([FromQuery] ResolveEmployeePoliciesRequestDTO dto) => Ok(await mediator.Send(new ResolveEmployeePoliciesQuery(dto)));

    [HttpPost("assignments")]
    public async Task<IActionResult> Assign([FromBody] AssignPolicyRequestDTO dto) => Ok(await mediator.Send(new AssignPolicyCommand(dto)));

    [HttpDelete("assignments/{assignmentId:long}")]
    public async Task<IActionResult> RemoveAssignment(long assignmentId, [FromQuery] RemovePolicyAssignmentRequestDTO dto)
    {
        dto.AssignmentId = assignmentId;
        return Ok(await mediator.Send(new RemovePolicyAssignmentCommand(dto)));
    }

    [HttpPost("exceptions")]
    public async Task<IActionResult> CreateException([FromBody] CreatePolicyExceptionRequestDTO dto) => Ok(await mediator.Send(new CreatePolicyExceptionCommand(dto)));

    [HttpPost("exceptions/{exceptionId:long}/decision")]
    public async Task<IActionResult> DecideException(long exceptionId, [FromBody] ApprovePolicyExceptionRequestDTO dto)
    {
        dto.ExceptionId = exceptionId;
        return Ok(await mediator.Send(new ApprovePolicyExceptionCommand(dto)));
    }

    [HttpPost("acknowledgements")]
    public async Task<IActionResult> Acknowledge([FromBody] AcknowledgePolicyRequestDTO dto) => Ok(await mediator.Send(new AcknowledgePolicyCommand(dto)));

    [HttpPost("documents")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument([FromForm] UploadPolicyDocumentRequestDTO dto) => Ok(await mediator.Send(new UploadPolicyDocumentCommand(dto)));

    [HttpGet("versions/{versionId:long}/documents")]
    public async Task<IActionResult> GetDocuments(long versionId, [FromQuery] PolicyDocumentsRequestDTO dto)
    {
        dto.PolicyVersionId = versionId;
        return Ok(await mediator.Send(new GetPolicyDocumentsQuery(dto)));
    }

    [HttpDelete("documents/{documentId:long}")]
    public async Task<IActionResult> DeleteDocument(long documentId, [FromQuery] DeletePolicyDocumentRequestDTO dto)
    {
        dto.DocumentId = documentId;
        return Ok(await mediator.Send(new DeletePolicyDocumentCommand(dto)));
    }

    [HttpGet("{policyId:long}/audit")]
    public async Task<IActionResult> GetAudit(long policyId, [FromQuery] PolicyByIdRequestDTO dto)
    {
        dto.Id = policyId;
        return Ok(await mediator.Send(new GetPolicyAuditQuery(dto)));
    }

    [HttpGet("approval-stages")]
    public async Task<IActionResult> GetApprovalStages([FromQuery] PolicyApprovalStageListRequestDTO dto) =>
        Ok(await mediator.Send(new GetPolicyApprovalStagesQuery(dto)));

    [HttpPost("approval-stages")]
    public async Task<IActionResult> CreateApprovalStage([FromBody] CreatePolicyApprovalStageRequestDTO dto) =>
        Ok(await mediator.Send(new CreatePolicyApprovalStageCommand(dto)));

    [HttpPut("approval-stages/{id:long}")]
    public async Task<IActionResult> UpdateApprovalStage(long id, [FromBody] UpdatePolicyApprovalStageRequestDTO dto)
    {
        dto.Id = id;
        return Ok(await mediator.Send(new UpdatePolicyApprovalStageCommand(dto)));
    }

    [HttpDelete("approval-stages/{id:long}")]
    public async Task<IActionResult> DeleteApprovalStage(long id, [FromQuery] DeletePolicyApprovalStageRequestDTO dto)
    {
        dto.Id = id;
        return Ok(await mediator.Send(new DeletePolicyApprovalStageCommand(dto)));
    }

    [HttpGet("versions/{versionId:long}/approval-progress")]
    public async Task<IActionResult> GetApprovalProgress(long versionId, [FromQuery] PolicyVersionAccessRequestDTO dto)
    {
        dto.PolicyVersionId = versionId;
        return Ok(await mediator.Send(new GetPolicyApprovalProgressQuery(dto)));
    }

    [HttpGet("versions/{versionId:long}/assignments")]
    public async Task<IActionResult> GetAssignments(long versionId, [FromQuery] PolicyVersionAccessRequestDTO dto)
    {
        dto.PolicyVersionId = versionId;
        return Ok(await mediator.Send(new GetPolicyAssignmentsQuery(dto)));
    }

    [HttpGet("versions/{versionId:long}/exceptions")]
    public async Task<IActionResult> GetExceptions(long versionId, [FromQuery] PolicyVersionAccessRequestDTO dto)
    {
        dto.PolicyVersionId = versionId;
        return Ok(await mediator.Send(new GetPolicyExceptionsQuery(dto)));
    }

    [HttpGet("versions/{versionId:long}/acknowledgements")]
    public async Task<IActionResult> GetAcknowledgements(long versionId, [FromQuery] PolicyVersionAccessRequestDTO dto)
    {
        dto.PolicyVersionId = versionId;
        return Ok(await mediator.Send(new GetPolicyAcknowledgementsQuery(dto)));
    }

    [HttpPost("bulk/{target}/preview")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(BulkImportConstants.MaxFileBytes + 65536)]
    public async Task<IActionResult> PreviewBulk(string target, [FromForm] BulkImportPreviewRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new PreviewPolicyBulkImportCommand(ParseBulkTarget(target), dto), cancellationToken));
    }

    [HttpPost("bulk/{target}/confirm")]
    public async Task<IActionResult> ConfirmBulk(string target, [FromBody] BulkImportJobRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ManagePolicyBulkImportCommand(ParseBulkTarget(target),
            BulkImportAction.Confirm, dto), cancellationToken));
    }

    [HttpGet("bulk/{target}/jobs/{jobId:guid}")]
    public async Task<IActionResult> GetBulkJob(string target, Guid jobId, [FromQuery] BulkImportJobRequestDTO dto,
        CancellationToken cancellationToken)
    {
        dto.JobId = jobId;
        return Ok(await mediator.Send(new ManagePolicyBulkImportCommand(ParseBulkTarget(target),
            BulkImportAction.Get, dto), cancellationToken));
    }

    [HttpGet("bulk/{target}/jobs")]
    public async Task<IActionResult> ListBulkJobs(string target, [FromQuery] BulkImportJobRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ManagePolicyBulkImportCommand(ParseBulkTarget(target),
            BulkImportAction.List, dto), cancellationToken));
    }

    [HttpPost("bulk/{target}/retry")]
    public async Task<IActionResult> RetryBulk(string target, [FromBody] BulkImportJobRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ManagePolicyBulkImportCommand(ParseBulkTarget(target),
            BulkImportAction.Retry, dto), cancellationToken));
    }

    [HttpPost("bulk/{target}/cancel")]
    public async Task<IActionResult> CancelBulk(string target, [FromBody] BulkImportJobRequestDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ManagePolicyBulkImportCommand(ParseBulkTarget(target),
            BulkImportAction.Cancel, dto), cancellationToken));
    }

    [HttpGet("bulk/{target}/template")]
    public async Task<IActionResult> GetBulkTemplate(string target, [FromQuery] BulkImportJobRequestDTO dto,
        CancellationToken cancellationToken)
    {
        var master = ParseBulkTarget(target);
        var response = await mediator.Send(new ManagePolicyBulkImportCommand(master,
            BulkImportAction.Template, dto), cancellationToken);
        return File(Encoding.UTF8.GetBytes((string)response.Data!), "text/csv; charset=utf-8",
            $"Policy-{target}-template.csv");
    }

    [HttpGet("bulk/{target}/jobs/{jobId:guid}/report")]
    public async Task<IActionResult> GetBulkReport(string target, Guid jobId, [FromQuery] BulkImportJobRequestDTO dto,
        CancellationToken cancellationToken)
    {
        dto.JobId = jobId;
        var response = await mediator.Send(new ManagePolicyBulkImportCommand(ParseBulkTarget(target),
            BulkImportAction.Get, dto), cancellationToken);
        return File(BulkImportReport.Create((BulkImportJobResponseDTO)response.Data!),
            "text/csv; charset=utf-8", $"Policy-{target}-import-{jobId}.csv");
    }

    private static BulkImportMaster ParseBulkTarget(string target)
    {
        return target.Trim().ToLowerInvariant() switch
        {
            "types" => BulkImportMaster.PolicyType,
            "definitions" => BulkImportMaster.PolicyDefinition,
            "assignments" => BulkImportMaster.PolicyAssignment,
            _ => throw new ValidationErrorException("Policy bulk target must be types, definitions or assignments.")
        };
    }
}
