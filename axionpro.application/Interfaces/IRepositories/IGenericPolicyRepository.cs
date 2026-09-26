using axionpro.application.DTOS.Policy;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

public interface IGenericPolicyRepository
{
    Task<IReadOnlyList<PolicyLookupResponseDTO>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyLookupResponseDTO>> GetStatusesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyLookupResponseDTO>> GetRuleTypesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyLookupResponseDTO>> GetDocumentTypesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyTypeResponseDTO>> GetPolicyTypesAsync(long tenantId, bool isActive, CancellationToken cancellationToken);
    Task<PolicyTypeResponseDTO> CreatePolicyTypeAsync(long tenantId, long actorId, CreateGenericPolicyTypeRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyTypeResponseDTO> UpdatePolicyTypeAsync(long tenantId, long actorId, UpdateGenericPolicyTypeRequestDTO dto, CancellationToken cancellationToken);
    Task<bool> ChangePolicyTypeStatusAsync(long tenantId, long actorId, ChangePolicyTypeStatusRequestDTO dto, CancellationToken cancellationToken);
    Task<(IReadOnlyList<PolicySummaryResponseDTO> Items, int Total)> GetPoliciesAsync(long tenantId, PolicyListRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyDetailResponseDTO> GetPolicyAsync(long tenantId, long policyId, CancellationToken cancellationToken);
    Task<PolicyDetailResponseDTO> CreatePolicyAsync(long tenantId, long actorId, CreatePolicyRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyDetailResponseDTO> UpdateDraftAsync(long tenantId, long actorId, UpdatePolicyDraftRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyDetailResponseDTO> CloneVersionAsync(long tenantId, long actorId, ClonePolicyVersionRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyDetailResponseDTO> TransitionAsync(long tenantId, long actorId, PolicyTransitionRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyAssignmentResultDTO> AssignAsync(long tenantId, long actorId, AssignPolicyRequestDTO dto, CancellationToken cancellationToken);
    Task<bool> RemoveAssignmentAsync(long tenantId, long actorId, long assignmentId, CancellationToken cancellationToken);
    Task<long> CreateExceptionAsync(long tenantId, long actorId, CreatePolicyExceptionRequestDTO dto, CancellationToken cancellationToken);
    Task<bool> ApproveExceptionAsync(long tenantId, long actorId, ApprovePolicyExceptionRequestDTO dto, CancellationToken cancellationToken);
    Task<bool> AcknowledgeAsync(long tenantId, long employeeId, AcknowledgePolicyRequestDTO dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<ResolvedPolicyResponseDTO>> ResolveAsync(long tenantId, long employeeId, ResolveEmployeePoliciesRequestDTO dto, CancellationToken cancellationToken);
    Task<long> AddDocumentAsync(long tenantId, long actorId, UploadPolicyDocumentRequestDTO dto, string objectKey, string checksum, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyDocument>> GetDocumentsAsync(long tenantId, long policyVersionId, CancellationToken cancellationToken);
    Task<string> DeleteDocumentAsync(long tenantId, long actorId, long documentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyAuditResponseDTO>> GetAuditAsync(long tenantId, long policyId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyApprovalStageResponseDTO>> GetApprovalStagesAsync(long tenantId, PolicyApprovalStageListRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyApprovalStageResponseDTO> CreateApprovalStageAsync(long tenantId, long actorId, CreatePolicyApprovalStageRequestDTO dto, CancellationToken cancellationToken);
    Task<PolicyApprovalStageResponseDTO> UpdateApprovalStageAsync(long tenantId, long actorId, UpdatePolicyApprovalStageRequestDTO dto, CancellationToken cancellationToken);
    Task<bool> DeleteApprovalStageAsync(long tenantId, long actorId, long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyApprovalProgressResponseDTO>> GetApprovalProgressAsync(long tenantId, long policyVersionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyAssignmentResponseDTO>> GetAssignmentsAsync(long tenantId, long policyVersionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyExceptionResponseDTO>> GetExceptionsAsync(long tenantId, long policyVersionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyAcknowledgementResponseDTO>> GetAcknowledgementsAsync(long tenantId, long policyVersionId, CancellationToken cancellationToken);
}
