using System.ComponentModel.DataAnnotations;
using axionpro.application.DTOs.BaseDTO;
using axionpro.domain.Entity;
using Microsoft.AspNetCore.Http;

namespace axionpro.application.DTOS.Policy;

public sealed class PolicyAccessRequestDTO : PermissionRequestDTO
{
    public bool IsActive { get; set; } = true;
}

public sealed class PolicyListRequestDTO : PermissionRequestDTO
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? PolicyTypeId { get; set; }
    public short? StatusId { get; set; }
    public string? Search { get; set; }
}

public sealed class PolicyByIdRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }
}

public class CreateGenericPolicyTypeRequestDTO : PermissionRequestDTO
{
    [Required, MaxLength(50)] public string PolicyTypeCode { get; set; } = null!;
    [Required, MaxLength(150)] public string PolicyName { get; set; } = null!;
    [MaxLength(500)] public string? Description { get; set; }
    [Range(1, int.MaxValue)] public int PolicyCategoryId { get; set; }
    [StringLength(3, MinimumLength = 3)] public string? DefaultCurrencyCode { get; set; }
}

public sealed class UpdateGenericPolicyTypeRequestDTO : CreateGenericPolicyTypeRequestDTO
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class ChangePolicyTypeStatusRequestDTO : PermissionRequestDTO
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePolicyRequestDTO : PermissionRequestDTO
{
    [Range(1, int.MaxValue)] public int PolicyTypeId { get; set; }
    [Required, MaxLength(50)] public string PolicyCode { get; set; } = null!;
    [Required, MaxLength(200)] public string PolicyName { get; set; } = null!;
    [MaxLength(1000)] public string? Summary { get; set; }
    public int? OwnerDepartmentId { get; set; }
    [StringLength(3, MinimumLength = 3)] public string? DefaultCurrencyCode { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    [MaxLength(1000)] public string? ChangeSummary { get; set; }
    public List<PolicyRuleInputDTO> Rules { get; set; } = new();
    public List<PolicyApplicabilityInputDTO> Applicability { get; set; } = new();
    public AttendancePolicyVersionConfigurationDTO? AttendanceConfiguration { get; set; }
}

public sealed class AttendancePolicyVersionConfigurationDTO
{
    public AttendanceLocationScope AttendanceLocationScope { get; set; }
    public bool AllowBiometric { get; set; }
    public bool AllowMobile { get; set; }
    public bool AllowWeb { get; set; }
    public bool AllowManualAttendance { get; set; }
    public bool AllowWorkFromHome { get; set; }
    public bool RequireGeoFenceForOffice { get; set; }
    public bool RequireGpsForRemote { get; set; }
    public bool AllowOutsideLocationWithApproval { get; set; }
}

public sealed class UpdatePolicyDraftRequestDTO : CreatePolicyRequestDTO
{
    public long PolicyId { get; set; }
    public long PolicyVersionId { get; set; }
}

public sealed class PolicyRuleInputDTO
{
    [Range(1, int.MaxValue)] public int PolicyRuleTypeId { get; set; }
    [Required, MaxLength(150)] public string RuleName { get; set; } = null!;
    [Range(1, int.MaxValue)] public int RuleOrder { get; set; }
    [Required] public string RuleConfiguration { get; set; } = "{}";
}

public sealed class PolicyApplicabilityInputDTO
{
    [Range(1, 2)] public short ApplicabilityMode { get; set; } = 1;
    public int? CountryId { get; set; }
    public int? StateId { get; set; }
    public int? DistrictId { get; set; }
    public int? LocalityId { get; set; }
    public long? TenantLocationId { get; set; }
    public int? EmployeeTypeId { get; set; }
    public int? DepartmentId { get; set; }
    public int? DesignationId { get; set; }
    public long? EmployeeId { get; set; }
    public int? GenderId { get; set; }
    public short? WorkArrangementType { get; set; }
    public short? EmploymentStatus { get; set; }
    [Range(0, int.MaxValue)] public int? MinimumServiceDays { get; set; }
    public int Priority { get; set; } = 100;
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}

public sealed class ClonePolicyVersionRequestDTO : PermissionRequestDTO
{
    public long PolicyId { get; set; }
    [Range(1, long.MaxValue)] public long SourceVersionId { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    [MaxLength(1000)] public string? ChangeSummary { get; set; }
}

public sealed class PolicyTransitionRequestDTO : PermissionRequestDTO
{
    public long PolicyVersionId { get; set; }
    [Required, MaxLength(20)] public string Action { get; set; } = null!;
    [MaxLength(1000)] public string? Comments { get; set; }
}

public sealed class ResolveEmployeePoliciesRequestDTO : PermissionRequestDTO
{
    [Range(1, long.MaxValue)] public long EmployeeId { get; set; }
    public DateOnly? EffectiveDate { get; set; }
}

public sealed class AssignPolicyRequestDTO : PermissionRequestDTO
{
    [Range(1, long.MaxValue)] public long PolicyVersionId { get; set; }
    [MinLength(1)] public List<long> EmployeeIds { get; set; } = new();
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsMandatory { get; set; } = true;
}

public sealed class RemovePolicyAssignmentRequestDTO : PermissionRequestDTO
{
    public long AssignmentId { get; set; }
}

public sealed class CreatePolicyExceptionRequestDTO : PermissionRequestDTO
{
    [Range(1, long.MaxValue)] public long PolicyVersionId { get; set; }
    [Range(1, long.MaxValue)] public long EmployeeId { get; set; }
    public short ExceptionType { get; set; }
    [Required] public string OverrideConfiguration { get; set; } = "{}";
    [Required, MaxLength(1000)] public string Reason { get; set; } = null!;
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly EffectiveTo { get; set; }
}

public sealed class ApprovePolicyExceptionRequestDTO : PermissionRequestDTO
{
    public long ExceptionId { get; set; }
    public bool Approve { get; set; }
}

public sealed class AcknowledgePolicyRequestDTO : PermissionRequestDTO
{
    [Range(1, long.MaxValue)] public long PolicyVersionId { get; set; }
    public string? EvidenceJson { get; set; }
}

public sealed class UploadPolicyDocumentRequestDTO : PermissionRequestDTO
{
    [Range(1, long.MaxValue)] public long PolicyVersionId { get; set; }
    [Range(1, short.MaxValue)] public short PolicyDocumentTypeId { get; set; }
    [Required, MaxLength(200)] public string DocumentTitle { get; set; } = null!;
    [MaxLength(10)] public string? LanguageCode { get; set; }
    public bool IsEmployeeVisible { get; set; } = true;
    [Required] public IFormFile File { get; set; } = null!;
}

public sealed class PolicyDocumentsRequestDTO : PermissionRequestDTO
{
    public long PolicyVersionId { get; set; }
}

public sealed class DeletePolicyDocumentRequestDTO : PermissionRequestDTO
{
    public long DocumentId { get; set; }
}

public sealed class PolicyVersionAccessRequestDTO : PermissionRequestDTO
{
    public long PolicyVersionId { get; set; }
}

public sealed class PolicyApprovalStageListRequestDTO : PermissionRequestDTO
{
    public int? PolicyCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreatePolicyApprovalStageRequestDTO : PermissionRequestDTO
{
    public int? PolicyCategoryId { get; set; }
    [Required, MaxLength(100)] public string StageName { get; set; } = null!;
    [Range(1, int.MaxValue)] public int StageOrder { get; set; }
    public int? ApproverRoleId { get; set; }
    [Range(1, int.MaxValue)] public int MinimumApprovals { get; set; } = 1;
    public bool IsMandatory { get; set; } = true;
}

public sealed class UpdatePolicyApprovalStageRequestDTO : CreatePolicyApprovalStageRequestDTO
{
    public long Id { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class DeletePolicyApprovalStageRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }
}

public sealed record PolicyLookupResponseDTO(int Id, string Code, string Name, string? Description = null);
public sealed record PolicyTypeResponseDTO(int Id, string Code, string Name, string? Description, int? CategoryId, string? CurrencyCode, bool IsActive);
public sealed record PolicySummaryResponseDTO(long Id, string Code, string Name, int PolicyTypeId, bool IsActive, long? CurrentVersionId, int? VersionNumber, string? Status);
public sealed record PolicyRuleResponseDTO(long Id, int RuleTypeId, string Name, int Order, string Configuration);
public sealed record PolicyApplicabilityResponseDTO(long Id, short Mode, int Priority,
    int? CountryId, int? StateId, int? DistrictId, int? LocalityId, long? TenantLocationId,
    int? EmployeeTypeId, int? DepartmentId, int? DesignationId, long? EmployeeId,
    int? GenderId, short? WorkArrangementType, short? EmploymentStatus,
    int? MinimumServiceDays, DateOnly EffectiveFrom, DateOnly? EffectiveTo);
public sealed record PolicyDetailResponseDTO(long Id, string Code, string Name, string? Summary,
    int PolicyTypeId, int? OwnerDepartmentId, string? DefaultCurrencyCode,
    long VersionId, int VersionNumber, short StatusId, string Status,
    DateOnly EffectiveFrom, DateOnly? EffectiveTo, string? ChangeSummary,
    IReadOnlyList<PolicyRuleResponseDTO> Rules,
    IReadOnlyList<PolicyApplicabilityResponseDTO> Applicability,
    AttendancePolicyVersionConfigurationDTO? AttendanceConfiguration);
public sealed record PolicyAssignmentResultDTO(int Inserted, int Existing);
public sealed record ResolvedPolicyResponseDTO(long PolicyId, long PolicyVersionId, string PolicyCode, string PolicyName, int Priority, string ResolutionSource);
public sealed record PolicyDocumentResponseDTO(long Id, long PolicyVersionId, short DocumentTypeId, string Title, string OriginalFileName, string ContentType, long FileSizeBytes, string? LanguageCode, bool IsEmployeeVisible, string Url);
public sealed record PolicyAuditResponseDTO(long Id, long PolicyId, long? PolicyVersionId, string EntityName, long? EntityId, string ActionName, string? BeforeData, string? AfterData, long ChangedById, DateTime ChangedDateTime, Guid? CorrelationId);
public sealed record PolicyApprovalStageResponseDTO(long Id, int? PolicyCategoryId, string StageName, int StageOrder, int? ApproverRoleId, int MinimumApprovals, bool IsMandatory, bool IsActive);
public sealed record PolicyApprovalProgressResponseDTO(long StageId, string StageName, int StageOrder, int MinimumApprovals, int ApprovalCount, bool IsComplete);
public sealed record PolicyAssignmentResponseDTO(long Id, long PolicyVersionId, long EmployeeId, short AssignmentSource, DateOnly EffectiveFrom, DateOnly? EffectiveTo, bool IsMandatory, bool IsActive);
public sealed record PolicyExceptionResponseDTO(long Id, long PolicyVersionId, long EmployeeId, short ExceptionType, string OverrideConfiguration, string Reason, DateOnly EffectiveFrom, DateOnly EffectiveTo, short ApprovalStatusId, bool IsActive);
public sealed record PolicyAcknowledgementResponseDTO(long Id, long PolicyVersionId, long EmployeeId, short Status, DateTime AssignedDateTime, DateTime? ViewedDateTime, DateTime? AcknowledgedDateTime);
