using System.Reflection;
using axionpro.api.Controllers.EmailTemplate;
using axionpro.application.Constants;
using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.Features.TenantEmailTemplateCmd;
using axionpro.domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("TenantEmailTemplate")]
public sealed class TenantEmailTemplateContractTests
{
    [Test]
    public void Entity_copies_email_template_fields_and_adds_tenant_id()
    {
        var hostProperties = typeof(EmailTemplate).GetProperties().Select(x => x.Name)
            .Where(x => x != nameof(EmailTemplate.EmailQueue)).ToHashSet();
        var tenantProperties = typeof(TenantEmailTemplate).GetProperties().Select(x => x.Name).ToHashSet();

        Assert.Multiple(() =>
        {
            Assert.That(tenantProperties, Does.Contain(nameof(TenantEmailTemplate.TenantId)));
            Assert.That(hostProperties.All(tenantProperties.Contains), Is.True);
        });
    }

    [Test]
    public void Controller_exposes_authorized_matching_crud_routes()
    {
        var type = typeof(TenantEmailTemplateController);
        var routes = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Select(x => x.GetCustomAttribute<HttpMethodAttribute>())
            .Where(x => x is not null)
            .Select(x => $"{x!.HttpMethods.Single()}:{x.Template}")
            .ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(type.GetCustomAttribute<AuthorizeAttribute>(), Is.Not.Null);
            Assert.That(routes, Does.Contain("POST:create"));
            Assert.That(routes, Does.Contain("GET:get-all"));
            Assert.That(routes, Does.Contain("GET:get-by-id/{id:int}"));
            Assert.That(routes, Does.Contain("POST:update"));
            Assert.That(routes, Does.Contain("POST:update-status"));
            Assert.That(routes, Does.Contain("POST:sync"));
            Assert.That(routes, Does.Contain("DELETE:delete/{id:int}"));
        });
    }

    [Test]
    public void Permission_and_repository_are_tenant_scoped()
    {
        var permission = Read("axionpro.application", "Features", "TenantEmailTemplateCmd", "TenantEmailTemplatePermissionBehavior.cs");
        var repository = Read("axionpro.persistance", "Repositories", "TenantEmailTemplateRepository.cs");

        Assert.Multiple(() =>
        {
            Assert.That(permission, Does.Contain("TENANT_EMAIL_TEMPLATE"));
            Assert.That(permission, Does.Contain("CheckTenantEmployeePermissionAsync"));
            Assert.That(repository, Does.Contain("x.TenantId == tenantId"));
            Assert.That(repository, Does.Not.Contain("context.EmailTemplates"));
        });
    }

    [Test]
    public void Production_seed_contains_module_operations_plan_entitlements_and_data_copy()
    {
        var seed = Read("database-scripts", "complete seed data", "AxionPro_New_Production_Module_Operation_Seed.sql");

        Assert.Multiple(() =>
        {
            Assert.That(seed, Does.Contain("'TENANT_EMAIL_TEMPLATE'"));
            Assert.That(seed, Does.Contain("parent.\"Id\" = 46"));
            Assert.That(seed, Does.Contain("ModuleOperationMapping"));
            Assert.That(seed, Does.Contain("PlanModuleMapping"));
            Assert.That(seed, Does.Contain("TenantEnabledModule"));
            Assert.That(seed, Does.Contain("TenantEnabledOperation"));
            Assert.That(seed, Does.Contain("CROSS JOIN axionpro.\"EmailTemplate\""));
        });
    }

    [TestCase("WELCOME_EMAIL", true)]
    [TestCase("forgot_password", true)]
    [TestCase("TENANT_FREE_TEXT_CODE", false)]
    [TestCase("", false)]
    public void Tenant_template_codes_are_limited_to_shared_predefined_constants(
        string templateCode,
        bool expected)
    {
        Assert.That(ConstantValues.IsSupportedEmailTemplateCode(templateCode), Is.EqualTo(expected));
    }

    [Test]
    public void Mail_delivery_resolves_tenant_template_and_keeps_host_fallback()
    {
        var repository = Read("axionpro.persistance", "Repositories", "TenantEmailTemplateRepository.cs");
        var service = Read("axionpro.infrastructure", "MailService", "EmailService.cs");

        Assert.Multiple(() =>
        {
            Assert.That(repository, Does.Contain("GetActiveByCodeAsync"));
            Assert.That(repository, Does.Contain("template.TenantId == tenantId"));
            Assert.That(repository, Does.Contain("template.IsActive"));
            Assert.That(service, Does.Contain("_tenantTemplateRepository.GetActiveByCodeAsync"));
            Assert.That(service, Does.Contain("_templateRepository.GetTemplateByCodeAsync"));
            Assert.That(service, Does.Contain("falling back to the Host template"));
        });
    }

    [Test]
    public void Queue_worker_keeps_tenant_code_retry_and_atomic_claim_contract()
    {
        var worker = Read("axionpro.infrastructure", "BackgroundJob", "EmailQueueWorker.cs");
        var repository = Read("axionpro.persistance", "Repositories", "EmailQueueRepository.cs");
        var migration = Read("database-scripts", "AlterEmailQueueForTenantTemplateResolution.sql");

        Assert.Multiple(() =>
        {
            Assert.That(worker, Does.Contain("SendTemplatedEmailAsync("));
            Assert.That(worker, Does.Contain("templateCode,"));
            Assert.That(worker, Does.Contain("item.ToEmail,"));
            Assert.That(worker, Does.Contain("item.TenantId,"));
            Assert.That(worker, Does.Contain("MaximumRetries = 5"));
            Assert.That(repository, Does.Contain("FOR UPDATE SKIP LOCKED"));
            Assert.That(repository, Does.Contain("ProcessingStartedDateTime"));
            Assert.That(migration, Does.Contain("\"TenantId\" bigint"));
            Assert.That(migration, Does.Contain("\"TemplateCode\" character varying(100)"));
            Assert.That(migration, Does.Contain("\"PlaceholdersJson\" text"));
        });
    }

    private static string Read(params string[] parts)
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AxionPro.sln")))
        {
            directory = directory.Parent;
        }

        Assert.That(directory, Is.Not.Null);
        return File.ReadAllText(Path.Combine(new[] { directory!.FullName }.Concat(parts).ToArray()));
    }
}
