using System.Reflection;
using axionpro.application.Features.EmployeeTypeCmd;
using axionpro.application.Features.EmployeeTypeCmd.Handlers;
using axionpro.application.Common.Models.Security;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.Exceptions;
using axionpro.application.Features.DepartmentCmd;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Features.DesignationCmd;
using axionpro.application.Features.DesignationCmd.Handlers;
using axionpro.application.Features.RoleCmd;
using axionpro.application.Features.RoleCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("BulkImport")]
public sealed class BulkImportPermissionTests
{
    [TestCase(BulkImportMaster.Department, "DepartmentName,Description,Remark,IsActive")]
    [TestCase(BulkImportMaster.Designation, "DesignationName,DepartmentName,Description,IsActive")]
    [TestCase(BulkImportMaster.Role, "RoleName,RoleType,Remark,IsActive")]
    [TestCase(BulkImportMaster.EmployeeType, "TypeName,Description,Remark,IsActive")]
    public async Task Granted_view_downloads_the_correct_template_without_writing_jobs(
        BulkImportMaster master, string expectedHeader)
    {
        var operations = Proxy<IOperationRepository>((_, _) =>
            Task.FromResult<axionpro.domain.Entity.Operation?>(new axionpro.domain.Entity.Operation
            {
                Id = 4,
                OperationName = "View",
                OperationType = (int)OperationType.View,
                IsActive = true
            }));
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name == "get_OperationRepository"
            ? operations : throw new InvalidOperationException(method.Name));
        var common = Proxy<ICommonRequestService>((_, _) => Task.FromResult(new CommonDecodedResult
        {
            Success = true, TenantId = 9, LoggedInEmployeeId = 44, RoleId = 7
        }));
        var repository = Proxy<IBulkImportRepository>((method, _) =>
            throw new InvalidOperationException("Template must not access job storage: " + method.Name));
        var service = new BulkImportWorkflowService(
            new BulkImportPreviewService(unit, common), repository, common, unit);

        var template = await service.ActAsync(master, BulkImportAction.Template,
            new BulkImportJobRequestDTO { OperationId = 4 }, CancellationToken.None);

        Assert.That(template, Is.EqualTo(expectedHeader + "\r\n"));
    }

    [Test]
    public void Result_report_preserves_quoted_multiline_values_and_neutralizes_formula_errors()
    {
        var job = new BulkImportJobResponseDTO
        {
            Preview = new BulkImportPreviewResponseDTO
            {
                Rows = new List<BulkImportPreviewRowDTO>
                {
                    new()
                    {
                        RowNumber = 3,
                        Status = BulkImportRowStatus.Failed,
                        Values = new Dictionary<string, string>
                        {
                            ["DepartmentName"] = "IT, \"Support\"\nTeam"
                        },
                        Errors = new List<string> { "=unsafe spreadsheet expression" }
                    }
                }
            }
        };
        var bytes = axionpro.api.Common.BulkImportReport.Create(job);
        using var stream = new MemoryStream(bytes);
        using var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(stream);
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        Assert.That(parser.ReadFields(), Is.EqualTo(new[]
        {
            "RowNumber", "Status", "RecordId", "Values", "Errors"
        }));
        var row = parser.ReadFields()!;
        Assert.Multiple(() =>
        {
            Assert.That(row.Length, Is.EqualTo(5));
            Assert.That(row[0], Is.EqualTo("3"));
            Assert.That(row[1], Is.EqualTo("Failed"));
            Assert.That(System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(row[3])!
                ["DepartmentName"], Is.EqualTo("IT, \"Support\"\nTeam"));
            Assert.That(row[4], Is.EqualTo("'=unsafe spreadsheet expression"));
        });
    }

    [TestCase(OperationType.View)]
    [TestCase(OperationType.Update)]
    [TestCase(OperationType.Delete)]
    public void Non_creation_operation_cannot_confirm_an_import(OperationType operationType)
    {
        var operations = Proxy<IOperationRepository>((_, _) => Task.FromResult<axionpro.domain.Entity.Operation?>(
            new axionpro.domain.Entity.Operation { Id = 4, OperationName = operationType.ToString(), OperationType = (int)operationType, IsActive = true }));
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name == "get_OperationRepository"
            ? operations : throw new InvalidOperationException(method.Name));
        var common = Proxy<ICommonRequestService>((_, _) => Task.FromResult(new CommonDecodedResult
        {
            Success = true, TenantId = 9, LoggedInEmployeeId = 44, RoleId = 7
        }));
        var repository = Proxy<IBulkImportRepository>((method, _) =>
            throw new InvalidOperationException("A forbidden operation reached the durable repository: " + method.Name));
        var service = new BulkImportWorkflowService(new BulkImportPreviewService(unit, common), repository, common, unit);
        Assert.ThrowsAsync<ForbiddenAccessException>(async () => await service.ActAsync(
            BulkImportMaster.Department, BulkImportAction.Confirm,
            new BulkImportJobRequestDTO { OperationId = 4, JobId = Guid.NewGuid() }, CancellationToken.None));
    }

    [TestCase(typeof(axionpro.api.Controllers.Department.DepartmentController))]
    [TestCase(typeof(axionpro.api.Controllers.Designation.DesignationController))]
    [TestCase(typeof(axionpro.api.Controllers.Role.RoleController))]
    [TestCase(typeof(axionpro.api.Controllers.EmployeeType.EmployeeTypeController))]
    public void Every_durable_route_is_explicit_and_authenticated(Type controller)
    {
        var routes = new Dictionary<string, string>
        {
            ["ConfirmBulkImport"] = "bulk/confirm",
            ["GetBulkImport"] = "bulk/jobs/{jobId:guid}",
            ["ListBulkImports"] = "bulk/jobs",
            ["RetryBulkImport"] = "bulk/retry",
            ["CancelBulkImport"] = "bulk/cancel",
            ["DownloadBulkTemplate"] = "bulk/template",
            ["DownloadBulkReport"] = "bulk/jobs/{jobId:guid}/report"
        };
        foreach (var route in routes)
        {
            var method = controller.GetMethod(route.Key)!;
            Assert.That(method.GetCustomAttribute<AuthorizeAttribute>(), Is.Not.Null, route.Key);
            Assert.That(method.GetCustomAttributes<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>()
                .Single().Template, Is.EqualTo(route.Value));
        }
    }

    [Test]
    public async Task Preview_reads_only_trusted_tenant_and_never_calls_a_write_method()
    {
        var departmentReads = 0;
        var departments = Proxy<IDepartmentRepository>((method, args) =>
        {
            Assert.That(method.Name, Is.EqualTo(nameof(IDepartmentRepository.GetAsync)));
            Assert.That(args![1], Is.EqualTo(9L));
            departmentReads++;
            return Task.FromResult(new PagedResponseDTO<GetDepartmentResponseDTO>
            {
                Data = new List<GetDepartmentResponseDTO>
                {
                    new() { Id = 10, DepartmentName = "IT", IsActive = true }
                }
            });
        });
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name == "get_DepartmentRepository"
            ? departments
            : throw new InvalidOperationException("Preview attempted an unexpected operation: " + method.Name));
        var common = Proxy<ICommonRequestService>((method, _) =>
            method.Name == nameof(ICommonRequestService.ValidateTenantUserRequestAsync)
                ? Task.FromResult(new CommonDecodedResult
                {
                    Success = true,
                    TenantId = 9,
                    LoggedInEmployeeId = 44,
                    RoleId = 7
                })
                : throw new InvalidOperationException(method.Name));
        var result = await new BulkImportPreviewService(unit, common).PreviewAsync(
            BulkImportMaster.Department,
            new BulkImportPreviewRequestDTO { PastedText = "DepartmentName\nIT\nFinance" },
            CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(departmentReads, Is.EqualTo(1));
            Assert.That(result.ExistingCount, Is.EqualTo(1));
            Assert.That(result.ReadyCount, Is.EqualTo(1));
            Assert.That(result.CanCommit, Is.False);
        });
    }

    [Test]
    public void Invalid_tenant_context_cannot_parse_or_read_master_data()
    {
        var unit = Proxy<IUnitOfWork>((method, _) =>
            throw new InvalidOperationException("Unauthorized repository access: " + method.Name));
        var common = Proxy<ICommonRequestService>((_, _) => Task.FromResult(new CommonDecodedResult
        {
            Success = false
        }));
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await new BulkImportPreviewService(unit, common).PreviewAsync(
                BulkImportMaster.Department,
                new BulkImportPreviewRequestDTO(),
                CancellationToken.None));
    }

    [TestCase("Department")]
    [TestCase("Designation")]
    [TestCase("Role")]
    [TestCase("EmployeeType")]
    public async Task Existing_pipeline_allows_granted_preview(string master)
    {
        await Check(master, wrongModule: false, allowed: true);
    }

    [TestCase("Department")]
    [TestCase("Designation")]
    [TestCase("Role")]
    [TestCase("EmployeeType")]
    public async Task Wrong_module_never_reaches_preview(string master)
    {
        await Check(master, wrongModule: true, allowed: true);
    }

    [TestCase("Department")]
    [TestCase("Designation")]
    [TestCase("Role")]
    [TestCase("EmployeeType")]
    public async Task Denied_permission_never_reaches_preview(string master)
    {
        await Check(master, wrongModule: false, allowed: false);
    }

    [TestCase(typeof(axionpro.api.Controllers.Department.DepartmentController))]
    [TestCase(typeof(axionpro.api.Controllers.Designation.DesignationController))]
    [TestCase(typeof(axionpro.api.Controllers.Role.RoleController))]
    [TestCase(typeof(axionpro.api.Controllers.EmployeeType.EmployeeTypeController))]
    public void Preview_route_requires_authentication_and_multipart_input(Type controller)
    {
        var action = controller.GetMethod("PreviewBulkImport")!;
        Assert.Multiple(() =>
        {
            Assert.That(action.GetCustomAttribute<AuthorizeAttribute>(), Is.Not.Null);
            Assert.That(action.GetCustomAttribute<HttpPostAttribute>()!.Template, Is.EqualTo("bulk/preview"));
            Assert.That(action.GetCustomAttribute<ConsumesAttribute>()!.ContentTypes, Does.Contain("multipart/form-data"));
        });
    }

    private static async Task Check(string master, bool wrongModule, bool allowed)
    {
        var moduleCode = master switch
        {
            "Department" => "TENANT_DEPARTMENTS",
            "Designation" => "TENANT_DESIGNATIONS",
            "EmployeeType" => "TENANT_EMPLOYEE_TYPES",
            _ => "TENANT_ROLES_PERMISSIONS"
        };
        var checks = 0;
        var store = Proxy<IStoreProcedureRepository>((method, args) =>
        {
            if (method.Name != nameof(IStoreProcedureRepository.CheckTenantEmployeePermissionAsync))
            {
                throw new InvalidOperationException(method.Name);
            }
            checks++;
            Assert.That(args![0], Is.EqualTo(9L));
            return Task.FromResult(new TenantsUserPermissionCheckResponseDTO { ResultCode = allowed ? 1 : 0 });
        });
        var unit = Proxy<IUnitOfWork>((method, _) => method.Name == "get_StoreProcedureRepository"
            ? store
            : throw new InvalidOperationException("Unexpected persistence access: " + method.Name));
        var common = Proxy<ICommonRequestService>((method, _) => method.Name switch
        {
            nameof(ICommonRequestService.GetModuleCodeAsync) => Task.FromResult<string?>(wrongModule ? "WRONG" : moduleCode),
            nameof(ICommonRequestService.ValidateTenantUserRequestAsync) => Task.FromResult(new CommonDecodedResult
            {
                Success = true,
                TenantId = 9,
                LoggedInEmployeeId = 44,
                RoleId = 7
            }),
            _ => throw new InvalidOperationException(method.Name)
        });
        var dto = new BulkImportPreviewRequestDTO { ModuleId = 25, OperationId = 1, PastedText = "Name\nIT" };
        var called = false;
        Task<ApiResponse<BulkImportPreviewResponseDTO>> Next(CancellationToken _)
        {
            called = true;
            return Task.FromResult(ApiResponse<BulkImportPreviewResponseDTO>.Success(new BulkImportPreviewResponseDTO()));
        }
        Task Run()
        {
            return master switch
            {
                "Department" => new DepartmentPermissionBehavior<PreviewDepartmentImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>(
                    unit, common, NullLogger<DepartmentPermissionBehavior<PreviewDepartmentImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>>.Instance)
                    .Handle(new PreviewDepartmentImportQuery(dto), Next, CancellationToken.None),
                "Designation" => new DesignationPermissionBehavior<PreviewDesignationImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>(
                    unit, common, NullLogger<DesignationPermissionBehavior<PreviewDesignationImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>>.Instance)
                    .Handle(new PreviewDesignationImportQuery(dto), Next, CancellationToken.None),
                "EmployeeType" => new EmployeeTypePermissionBehavior<PreviewEmployeeTypeImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>(
                    unit, common, NullLogger<EmployeeTypePermissionBehavior<PreviewEmployeeTypeImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>>.Instance)
                    .Handle(new PreviewEmployeeTypeImportQuery(dto), Next, CancellationToken.None),
                _ => new RolePermissionBehavior<PreviewRoleImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>(
                    unit, common, NullLogger<RolePermissionBehavior<PreviewRoleImportQuery, ApiResponse<BulkImportPreviewResponseDTO>>>.Instance)
                    .Handle(new PreviewRoleImportQuery(dto), Next, CancellationToken.None)
            };
        }
        if (wrongModule || !allowed)
        {
            Assert.ThrowsAsync<ForbiddenAccessException>(async () => await Run());
        }
        else
        {
            await Run();
        }
        Assert.Multiple(() =>
        {
            Assert.That(called, Is.EqualTo(!wrongModule && allowed));
            Assert.That(checks, Is.EqualTo(wrongModule ? 0 : 1));
        });
    }

    public class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> InvokeMethod { get; set; } = null!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            return InvokeMethod(targetMethod!, args);
        }
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }
}
