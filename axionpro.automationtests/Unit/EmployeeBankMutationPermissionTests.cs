using System.Reflection;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Exceptions;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeMutationPermission")]
public sealed class EmployeeBankMutationPermissionTests
{
    [TestCase(false, false, true)]
    [TestCase(true, true, true)]
    [TestCase(true, false, false)]
    public void Bank_delete_rejects_foreign_scope_verified_or_locked_record(
        bool scopeAllowed, bool verified, bool editable)
    {
        var bank = new EmployeeBankDetail
        {
            Id = 10, EmployeeId = 25, IsInfoVerified = verified, IsEditAllowed = editable
        };
        var mutationCalled = false;
        var repository = Proxy<IEmployeeBankRepository>((method, _) =>
        {
            if (method.Name == nameof(IEmployeeBankRepository.GetSingleRecordAsync))
                return Task.FromResult(bank);
            mutationCalled = true;
            throw new InvalidOperationException("Unauthorized mutation reached repository.");
        });
        var unitOfWork = Proxy<IUnitOfWork>((method, _) => method.Name == "get_EmployeeBankRepository"
            ? repository : throw new InvalidOperationException(method.Name));
        var common = Proxy<ICommonRequestService>((method, _) => method.Name switch
        {
            nameof(ICommonRequestService.ValidateTenantUserRequestAsync) => Task.FromResult(new CommonDecodedResult
            {
                Success = true, TenantId = 1, LoggedInEmployeeId = 25, UserEmployeeId = 25,
                RoleTypeId = ConstantValues.RoleTypeEmployee
            }),
            nameof(ICommonRequestService.CanAccessEmployeeDataAsync) => Task.FromResult(scopeAllowed),
            _ => throw new InvalidOperationException(method.Name)
        });
        var handler = new DeleteBankInfoQueryHandler(unitOfWork,
            NullLogger<DeleteBankInfoQueryHandler>.Instance, null!, common);

        Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(
            new DeleteBankInfoQuery(new DeleteBankRequestDTO { Id = 10 }), CancellationToken.None));
        Assert.That(mutationCalled, Is.False);
        Assert.That(bank.IsSoftDeleted, Is.Not.True);
    }

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, BulkImportPermissionTests.TestProxy>();
        ((BulkImportPermissionTests.TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }
}
