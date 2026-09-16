using System.Reflection;
using axionpro.api.Controllers.Policies;
using axionpro.application.DTOS.Policy;
using axionpro.application.Features.GenericPolicyCmd;
using axionpro.application.Common.Enums;
using axionpro.application.Constants;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("PolicyFramework")]
public sealed class GenericPolicyApiContractTests
{
    [Test]
    public void Route_supplied_identifiers_skip_pre_action_model_validation()
    {
        var routeProperties = new (Type Type, string Property)[]
        {
            (typeof(PolicyByIdRequestDTO), nameof(PolicyByIdRequestDTO.Id)),
            (typeof(UpdateGenericPolicyTypeRequestDTO), nameof(UpdateGenericPolicyTypeRequestDTO.Id)),
            (typeof(ChangePolicyTypeStatusRequestDTO), nameof(ChangePolicyTypeStatusRequestDTO.Id)),
            (typeof(UpdatePolicyDraftRequestDTO), nameof(UpdatePolicyDraftRequestDTO.PolicyId)),
            (typeof(UpdatePolicyDraftRequestDTO), nameof(UpdatePolicyDraftRequestDTO.PolicyVersionId)),
            (typeof(ClonePolicyVersionRequestDTO), nameof(ClonePolicyVersionRequestDTO.PolicyId)),
            (typeof(PolicyTransitionRequestDTO), nameof(PolicyTransitionRequestDTO.PolicyVersionId)),
            (typeof(RemovePolicyAssignmentRequestDTO), nameof(RemovePolicyAssignmentRequestDTO.AssignmentId)),
            (typeof(ApprovePolicyExceptionRequestDTO), nameof(ApprovePolicyExceptionRequestDTO.ExceptionId)),
            (typeof(PolicyDocumentsRequestDTO), nameof(PolicyDocumentsRequestDTO.PolicyVersionId)),
            (typeof(DeletePolicyDocumentRequestDTO), nameof(DeletePolicyDocumentRequestDTO.DocumentId)),
            (typeof(PolicyVersionAccessRequestDTO), nameof(PolicyVersionAccessRequestDTO.PolicyVersionId)),
            (typeof(UpdatePolicyApprovalStageRequestDTO), nameof(UpdatePolicyApprovalStageRequestDTO.Id)),
            (typeof(DeletePolicyApprovalStageRequestDTO), nameof(DeletePolicyApprovalStageRequestDTO.Id))
        };

        Assert.Multiple(() =>
        {
            foreach (var (type, propertyName) in routeProperties)
            {
                var property = type.GetProperty(propertyName);
                Assert.That(property, Is.Not.Null, $"{type.Name}.{propertyName}");
                Assert.That(property!.GetCustomAttributes<System.ComponentModel.DataAnnotations.ValidationAttribute>(), Is.Empty,
                    $"{type.Name}.{propertyName} is supplied by the route after automatic DTO validation.");
            }
        });
    }

    [Test]
    public void Ef_model_maps_all_generic_policy_tables_and_json_columns()
    {
        var options = new DbContextOptionsBuilder<WorkforceDbContext>().UseNpgsql("Host=localhost;Database=model_only;Username=x;Password=x").Options;
        using var context = new WorkforceDbContext(options);
        var expected = new[] { typeof(PolicyCategory), typeof(PolicyStatus), typeof(PolicyRuleType), typeof(PolicyDocumentType), typeof(PolicyType), typeof(Policy), typeof(PolicyVersion), typeof(PolicyRule), typeof(PolicyApplicability), typeof(PolicyAssignment), typeof(PolicyException), typeof(PolicyDocument), typeof(PolicyApprovalStage), typeof(PolicyApprovalHistory), typeof(PolicyAcknowledgement), typeof(PolicyChangeAudit) };
        Assert.Multiple(() =>
        {
            foreach (var type in expected) Assert.That(context.Model.FindEntityType(type), Is.Not.Null, type.Name);
            Assert.That(context.Model.FindEntityType(typeof(PolicyRule))!.FindProperty(nameof(PolicyRule.RuleConfiguration))!.GetColumnType(), Is.EqualTo("jsonb"));
            Assert.That(context.Model.FindEntityType(typeof(PolicyException))!.FindProperty(nameof(PolicyException.OverrideConfiguration))!.GetColumnType(), Is.EqualTo("jsonb"));
        });
    }

    [Test]
    public void Controller_exposes_policy_crud_lifecycle_resolution_assignment_exception_and_acknowledgement_routes()
    {
        var routes = typeof(TenantPolicyController).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>().Select(attribute => $"{attribute.HttpMethods.Single()}:{attribute.Template}"))
            .ToArray();
        Assert.Multiple(() =>
        {
            Assert.That(routes, Does.Contain("GET:lookups"));
            Assert.That(routes, Does.Contain("POST:"));
            Assert.That(routes, Does.Contain("PUT:{policyId:long}/versions/{versionId:long}"));
            Assert.That(routes, Does.Contain("POST:versions/{versionId:long}/transition"));
            Assert.That(routes, Does.Contain("GET:resolve"));
            Assert.That(routes, Does.Contain("POST:assignments"));
            Assert.That(routes, Does.Contain("POST:exceptions"));
            Assert.That(routes, Does.Contain("POST:acknowledgements"));
            Assert.That(routes, Does.Contain("GET:approval-stages"));
            Assert.That(routes, Does.Contain("POST:approval-stages"));
            Assert.That(routes, Does.Contain("GET:versions/{versionId:long}/approval-progress"));
            Assert.That(routes, Does.Contain("GET:versions/{versionId:long}/assignments"));
            Assert.That(routes, Does.Contain("GET:versions/{versionId:long}/exceptions"));
            Assert.That(routes, Does.Contain("GET:versions/{versionId:long}/acknowledgements"));
            Assert.That(routes, Does.Contain("POST:bulk/{target}/preview"));
            Assert.That(routes, Does.Contain("POST:bulk/{target}/confirm"));
            Assert.That(routes, Does.Contain("GET:bulk/{target}/jobs/{jobId:guid}"));
            Assert.That(routes, Does.Contain("GET:bulk/{target}/jobs"));
            Assert.That(routes, Does.Contain("POST:bulk/{target}/retry"));
            Assert.That(routes, Does.Contain("POST:bulk/{target}/cancel"));
            Assert.That(routes, Does.Contain("GET:bulk/{target}/template"));
            Assert.That(routes, Does.Contain("GET:bulk/{target}/jobs/{jobId:guid}/report"));
        });
    }

    [Test]
    public void Policy_bulk_targets_have_stable_master_values_and_exact_templates()
    {
        Assert.Multiple(() =>
        {
            Assert.That((int)BulkImportMaster.PolicyType, Is.EqualTo(12));
            Assert.That((int)BulkImportMaster.PolicyDefinition, Is.EqualTo(13));
            Assert.That((int)BulkImportMaster.PolicyAssignment, Is.EqualTo(14));
            Assert.That(BulkImportConstants.PolicyTypeColumns,
                Is.EqualTo(new[] { "PolicyTypeCode", "PolicyName", "PolicyCategoryCode", "Description", "DefaultCurrencyCode", "IsActive" }));
            Assert.That(BulkImportConstants.PolicyDefinitionColumns,
                Does.Contain("RulesJson").And.Contain("ApplicabilityJson"));
            Assert.That(BulkImportConstants.PolicyAssignmentColumns,
                Is.EqualTo(new[] { "PolicyCode", "VersionNumber", "EmployeeCode", "EffectiveFrom", "EffectiveTo", "IsMandatory" }));
        });
    }

    [Test]
    public void Policy_bulk_migration_allows_all_policy_masters_and_is_published()
    {
        var migration = ReadRepositoryFile("database-scripts", "AddPolicyBulkImport.sql");
        var runner = ReadRepositoryFile("database-scripts", "ApplyBulkImportMigrations.ps1");
        var project = ReadRepositoryFile("axionpro.api", "axionpro.api.csproj");
        Assert.Multiple(() =>
        {
            Assert.That(migration, Does.Contain("CHECK (\"Master\" BETWEEN 1 AND 14)"));
            Assert.That(runner, Does.Contain("[switch] $PolicyBulkOnly"));
            Assert.That(runner, Does.Contain("$scripts = @('AddPolicyBulkImport.sql')"));
            Assert.That(project, Does.Contain("AddPolicyBulkImport.sql"));
        });
    }

    [Test]
    public void Policy_bulk_permission_and_worker_are_bound_to_existing_leaf_modules()
    {
        var permission = ReadRepositoryFile("axionpro.application", "Features", "GenericPolicyCmd", "GenericPolicyPermissionBehavior.cs");
        var worker = ReadRepositoryFile("axionpro.persistance", "Repositories", "BulkImportRepository.cs");
        Assert.Multiple(() =>
        {
            Assert.That(permission, Does.Contain("BulkImportMaster.PolicyType => \"TENANT_POLICY_TYPES\""));
            Assert.That(permission, Does.Contain("BulkImportMaster.PolicyDefinition => \"TENANT_POLICY_DEFINITIONS\""));
            Assert.That(permission, Does.Contain("BulkImportMaster.PolicyAssignment => \"TENANT_POLICY_ASSIGNMENTS\""));
            Assert.That(worker, Does.Contain("await ProcessPolicyBatchAsync(job, preview, cancellationToken)"));
            Assert.That(worker, Does.Contain("BulkImportMaster.PolicyAssignment => BulkImportConstants.PolicyAssignmentModuleCode"));
        });
    }

    [Test]
    public void Policy_bulk_revalidates_rows_and_enforces_tenant_reference_boundaries()
    {
        var source = ReadRepositoryFile("axionpro.persistance", "Repositories", "BulkImportRepository.Policy.cs");
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("BuildPolicyPreviewAsync((BulkImportMaster)job.Master"));
            Assert.That(source, Does.Contain("RulesJson contains an inactive or unknown PolicyRuleTypeId."));
            Assert.That(source, Does.Contain("TenantLocationId outside the tenant or supplied geography."));
            Assert.That(source, Does.Contain("EmployeeTypeId for this tenant."));
            Assert.That(source, Does.Contain("PolicyCode and VersionNumber must identify a published tenant policy version."));
        });
    }

    [Test]
    public void Policy_bulk_definition_creates_draft_and_assignment_creates_acknowledgement()
    {
        var source = ReadRepositoryFile("axionpro.persistance", "Repositories", "BulkImportRepository.Policy.cs");
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("PolicyStatusId = 1"));
            Assert.That(source, Does.Contain("ActionName = \"BULK_CREATE\""));
            Assert.That(source, Does.Contain("context.PolicyAcknowledgements.Add"));
            Assert.That(source, Does.Contain("AcknowledgementStatus = 1"));
        });
    }

    [Test]
    public void Ui_catalog_documents_every_controller_operation_with_request_and_response_examples()
    {
        var catalog = ReadRepositoryFile("docs", "UIdeveloperDoc", "TENANT_POLICY_ENDPOINT_CATALOG.md");
        var methods = typeof(TenantPolicyController).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>())
            .ToArray();
        Assert.Multiple(() =>
        {
            Assert.That(methods, Has.Length.EqualTo(37));
            for (var number = 1; number <= methods.Length; number++)
            {
                Assert.That(catalog, Does.Contain($"### {number}."), $"Missing documented endpoint number {number}");
            }
            Assert.That(catalog, Does.Contain("Input body"));
            Assert.That(catalog, Does.Contain("Output sample"));
            Assert.That(catalog, Does.Contain("Current verification status"));
        });
    }

    [Test]
    public void Every_generic_policy_request_carries_dynamic_permission_ids()
    {
        var requestTypes = typeof(CreatePolicyRequestDTO).Assembly.GetTypes().Where(x => x.Namespace == typeof(CreatePolicyRequestDTO).Namespace && x.Name.EndsWith("RequestDTO", StringComparison.Ordinal)).ToArray();
        Assert.That(requestTypes, Is.Not.Empty);
        Assert.Multiple(() => { foreach (var type in requestTypes) { Assert.That(type.GetProperty("ModuleId"), Is.Not.Null, type.Name); Assert.That(type.GetProperty("OperationId"), Is.Not.Null, type.Name); } });
    }

    [Test]
    public void Lifecycle_contract_supports_only_documented_actions()
    {
        var source = ReadRepositoryFile("axionpro.persistance", "Repositories", "GenericPolicyRepository.cs");
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("(Draft, \"SUBMIT\") => UnderReview"));
            Assert.That(source, Does.Contain("RecordApprovalDecisionAsync"));
            Assert.That(source, Does.Contain("currentStage.MinimumApprovals"));
            Assert.That(source, Does.Contain("currentStage.ApproverRoleId"));
            Assert.That(source, Does.Contain("x.ActionById == actorId"));
            Assert.That(source, Does.Contain("(Approved, \"PUBLISH\") => Published"));
            Assert.That(source, Does.Contain("(Published, \"ARCHIVE\") => Archived"));
            Assert.That(source, Does.Contain("Only a published policy version can be assigned."));
        });
    }

    [Test]
    public void Repository_enforces_tenant_scope_on_employee_sensitive_operations()
    {
        var source = ReadRepositoryFile("axionpro.persistance", "Repositories", "GenericPolicyRepository.cs");
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("x.TenantId == tenantId && employeeIds.Contains(x.Id)"));
            Assert.That(source, Does.Contain("x.Id == dto.EmployeeId && x.TenantId == tenantId"));
            Assert.That(source, Does.Contain("x.Id == assignmentId && x.TenantId == tenantId"));
            Assert.That(source, Does.Contain("x.TenantId == tenantId && x.EmployeeId == employeeId"));
        });
    }

    [Test]
    public void Assignment_creates_acknowledgement_idempotently_and_reactivates_removed_assignment()
    {
        var source = ReadRepositoryFile("axionpro.persistance", "Repositories", "GenericPolicyRepository.cs");
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("inactive.IsActive = true"));
            Assert.That(source, Does.Contain("context.PolicyAcknowledgements.AddRange"));
            Assert.That(source, Does.Contain("validEmployeeIds.Except(acknowledgementEmployeeIds)"));
        });
    }

    [Test]
    public void Applicability_validates_hierarchy_and_uses_documented_precedence()
    {
        var source = ReadRepositoryFile("axionpro.persistance", "Repositories", "GenericPolicyRepository.cs");
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("StateId does not belong to CountryId."));
            Assert.That(source, Does.Contain("DistrictId does not belong to StateId."));
            Assert.That(source, Does.Contain("TenantLocationId does not match the supplied geography."));
            Assert.That(source, Does.Contain("var specificity = matching.Max(ScopeSpecificity)"));
            Assert.That(source, Does.Contain("var priority = mostSpecific.Min"));
            Assert.That(source, Does.Contain("winners.Any(x => x.ApplicabilityMode == 2)"));
        });
    }

    [Test]
    public void Permission_behavior_binds_each_action_group_to_its_leaf_module()
    {
        var source = ReadRepositoryFile("axionpro.application", "Features", "GenericPolicyCmd", "GenericPolicyPermissionBehavior.cs");
        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Contain("TENANT_POLICY_TYPES"));
            Assert.That(source, Does.Contain("TENANT_POLICY_DEFINITIONS"));
            Assert.That(source, Does.Contain("TENANT_POLICY_ASSIGNMENTS"));
            Assert.That(source, Does.Contain("TENANT_POLICY_EXCEPTIONS"));
            Assert.That(source, Does.Contain("TENANT_POLICY_APPROVALS"));
            Assert.That(source, Does.Contain("TENANT_POLICY_ACKNOWLEDGEMENTS"));
            Assert.That(source, Does.Contain("TENANT_POLICY_AUDIT"));
            Assert.That(source, Does.Contain("The selected module is not valid for this policy action."));
        });
    }

    private static string ReadRepositoryFile(params string[] parts)
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "AxionPro.sln"))) directory = directory.Parent;
        return File.ReadAllText(Path.Combine(new[] { directory?.FullName ?? throw new DirectoryNotFoundException() }.Concat(parts).ToArray()));
    }
}
