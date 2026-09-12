using axionpro.api.Middlewares;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.persistance.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using axionpro.persistance.Data.Context;

namespace axionpro.automationtests.Unit;

[TestFixture, Category("AuthenticationStatus")]
public sealed class AuthenticationStatusRegressionTests
{
    [TestCase(0, 403)]
    [TestCase(-1, 401)]
    [TestCase(-2, 401)]
    public async Task Tenant_permission_result_preserves_permission_vs_authentication_status(int result, int status)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ErrorHandlerMiddleware(_ =>
        {
            TenantRuntimePermissionValidator.EnsureAllowed(new TenantsUserPermissionCheckResponseDTO { ResultCode = result });
            return Task.CompletedTask;
        }, NullLogger<ErrorHandlerMiddleware>.Instance);
        await middleware.InvokeAsync(context);
        Assert.That(context.Response.StatusCode, Is.EqualTo(status));
    }

    [TestCase(0, 4)]
    [TestCase(78, 0)]
    [TestCase(78, -1)]
    public async Task Missing_action_identifiers_do_not_become_authentication_401(int moduleId, int operationId)
    {
        // No provider is configured: any accidental query fails before a connection is possible.
        await using var db = new WorkforceDbContext(new DbContextOptionsBuilder<WorkforceDbContext>().Options);
        var mapper = new MapperConfiguration(_ => { }).CreateMapper();
        var repository = new StoreProcedureRepository(db, NullLogger<StoreProcedureRepository>.Instance, mapper);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ErrorHandlerMiddleware(async _ =>
        {
            await repository.CheckTenantEmployeePermissionAsync(8, 50, 7, moduleId, operationId);
        }, NullLogger<ErrorHandlerMiddleware>.Instance);
        await middleware.InvokeAsync(context);
        Assert.That(context.Response.StatusCode, Is.EqualTo(400));
    }
}
