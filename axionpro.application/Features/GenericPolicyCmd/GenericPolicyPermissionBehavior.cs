using System.Reflection;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using MediatR;

namespace axionpro.application.Features.GenericPolicyCmd;

public sealed class GenericPolicyPermissionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (typeof(TRequest).Namespace?.StartsWith("axionpro.application.Features.GenericPolicyCmd", StringComparison.Ordinal) != true)
        {
            return await next();
        }
        var dto = typeof(TRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.GetValue(request)).OfType<PermissionRequestDTO>().FirstOrDefault()
            ?? throw new ValidationErrorException("Permission request is required.");
        if (dto.ModuleId <= 0 || dto.OperationId <= 0)
        {
            throw new ValidationErrorException("ModuleId and OperationId are required.");
        }
        var moduleCode = await commonRequestService.GetModuleCodeAsync(dto.ModuleId);
        var expectedModuleCode = ExpectedModuleCode(request);
        var isPolicyModule = moduleCode != null && (moduleCode.Equals("TENANT_POLICIES", StringComparison.OrdinalIgnoreCase)
            || moduleCode.StartsWith("TENANT_POLICY_", StringComparison.OrdinalIgnoreCase));
        if (!isPolicyModule || expectedModuleCode != null
            && !moduleCode!.Equals(expectedModuleCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException("The selected module is not valid for this policy action.");
        }
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0 || actor.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(actor.ErrorMessage ?? "Unauthorized request.");
        }
        var permission = await unitOfWork.StoreProcedureRepository.CheckTenantEmployeePermissionAsync(actor.TenantId, actor.LoggedInEmployeeId, actor.RoleId, dto.ModuleId, dto.OperationId, cancellationToken);
        TenantRuntimePermissionValidator.EnsureAllowed(permission);
        return await next();
    }

    private static string? ExpectedModuleCode(TRequest request)
    {
        return request switch
        {
            GetPolicyTypesQuery or CreatePolicyTypeCommand or UpdateGenericPolicyTypeCommand
                or ChangePolicyTypeStatusCommand => "TENANT_POLICY_TYPES",
            GetPoliciesQuery or GetPolicyQuery or CreatePolicyCommand or UpdatePolicyDraftCommand
                or ClonePolicyVersionCommand or ResolveEmployeePoliciesQuery or UploadPolicyDocumentCommand
                or GetPolicyDocumentsQuery or DeletePolicyDocumentCommand => "TENANT_POLICY_DEFINITIONS",
            TransitionPolicyCommand transition when transition.DTO.Action.Trim().Equals("SUBMIT", StringComparison.OrdinalIgnoreCase)
                => "TENANT_POLICY_DEFINITIONS",
            TransitionPolicyCommand => "TENANT_POLICY_APPROVALS",
            AssignPolicyCommand or RemovePolicyAssignmentCommand or GetPolicyAssignmentsQuery
                => "TENANT_POLICY_ASSIGNMENTS",
            CreatePolicyExceptionCommand or ApprovePolicyExceptionCommand or GetPolicyExceptionsQuery
                => "TENANT_POLICY_EXCEPTIONS",
            AcknowledgePolicyCommand or GetPolicyAcknowledgementsQuery
                => "TENANT_POLICY_ACKNOWLEDGEMENTS",
            GetPolicyAuditQuery => "TENANT_POLICY_AUDIT",
            GetPolicyApprovalStagesQuery or CreatePolicyApprovalStageCommand or UpdatePolicyApprovalStageCommand
                or DeletePolicyApprovalStageCommand or GetPolicyApprovalProgressQuery
                => "TENANT_POLICY_APPROVALS",
            PreviewPolicyBulkImportCommand bulk => BulkModuleCode(bulk.Master),
            ManagePolicyBulkImportCommand bulk => BulkModuleCode(bulk.Master),
            _ => null
        };
    }

    private static string BulkModuleCode(BulkImportMaster master)
    {
        return master switch
        {
            BulkImportMaster.PolicyType => "TENANT_POLICY_TYPES",
            BulkImportMaster.PolicyDefinition => "TENANT_POLICY_DEFINITIONS",
            BulkImportMaster.PolicyAssignment => "TENANT_POLICY_ASSIGNMENTS",
            _ => throw new ValidationErrorException("Unsupported policy import target.")
        };
    }
}
