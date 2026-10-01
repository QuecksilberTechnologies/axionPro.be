using System.Reflection;
using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Models.Security;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Location;
using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Mappings;
using axionpro.infrastructure.EncryptionService;
using axionpro.infrastructure.Security.HashedService;
using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeContactDatabase")]
[NonParallelizable]
public sealed class EmployeeContactDatabaseTests
{
    [Test]
    public async Task Uae_location_repository_returns_seeded_hierarchy_without_cross_country_rows()
    {
        var config = Configuration();
        await using var db = Database(config);
        var repository = new LocationRepository(db, NullLogger<LocationRepository>.Instance);
        var countryId = await db.Countries.Where(item => item.CountryCode == "AE").Select(item => item.Id).SingleAsync();
        var states = await repository.GetStateOptionAsync(new GetStateOptionRequestDTO { CountryId = countryId });
        Assert.That(states, Has.Count.EqualTo(7));
        var totalDistricts = 0;
        var totalLocalities = 0;
        foreach (var state in states)
        {
            Assert.That(state.CountryCode, Is.EqualTo("AE"));
            var districts = await repository.GetDistrictOptionAsync(new GetDistrictOptionRequestDTO { StateId = state.Id });
            Assert.That(districts, Is.Not.Empty);
            var stateLocalities = 0;
            foreach (var district in districts)
            {
                Assert.That(district.StateId, Is.EqualTo(state.Id));
                var localities = await repository.GetLocalityOptionAsync(new GetLocalityOptionRequestDTO { DistrictId = district.Id });
                Assert.That(localities.All(item => item.StateId == state.Id && item.DistrictId == district.Id), Is.True);
                Assert.That(localities.All(item => item.PostalCode == null), Is.True);
                stateLocalities += localities.Count;
            }
            Assert.That(stateLocalities, Is.GreaterThan(0), state.StateName);
            totalDistricts += districts.Count;
            totalLocalities += stateLocalities;
        }
        Assert.That(totalDistricts, Is.EqualTo(35));
        Assert.That(totalLocalities, Is.EqualTo(2776));
    }

    [Test]
    public async Task Employee_creation_contact_edit_and_add_persist_in_rollback_probe()
    {
        var config = Configuration();
        await using var db = Database(config);
        var template = await db.Employees.AsNoTracking()
            .FirstAsync(item => item.Country.CountryCode == "AE" && item.IsActive && !item.IsSoftDeleted);
        var roleId = await db.Roles.Where(item => item.TenantId == template.TenantId && item.IsActive && item.RoleType == 2)
            .Select(item => item.Id).FirstAsync();
        var initialEmployeeCount = await db.Employees.CountAsync();
        var initialContactCount = await db.EmployeeContacts.CountAsync();
        var mapper = new MapperConfiguration(options => options.AddProfile<MappingProfile>()).CreateMapper();
        var password = new PasswordService(NullLogger<PasswordService>.Instance);
        var encryption = new AesEncryptionService();
        // Encoding/auth/email are synthetic; this probe verifies real handlers/repositories, not HTTP JWT acceptance.
        var encoder = Proxy<IIdEncoderService>((method, args) => method.Name switch
        {
            "DecodeId_long" => long.Parse((string)args![0]!),
            "DecodeId_int" => int.Parse((string)args![0]!),
            _ => args![0]!.ToString()
        });
        var constructor = typeof(global::UnitOfWork).GetConstructors().Single();
        var dependencies = constructor.GetParameters().Select(parameter =>
        {
            object? value = parameter.ParameterType == typeof(WorkforceDbContext) ? db :
                parameter.ParameterType == typeof(ILoggerFactory) ? NullLoggerFactory.Instance :
                parameter.ParameterType == typeof(IMapper) ? mapper :
                parameter.ParameterType == typeof(IConfiguration) ? config :
                parameter.ParameterType == typeof(IEncryptionService) ? encryption :
                parameter.ParameterType == typeof(IIdEncoderService) ? encoder :
                parameter.ParameterType == typeof(IPasswordService) ? password : null;
            if (value is not null)
            {
                return value;
            }
            var dependency = DispatchProxy.Create(parameter.ParameterType, typeof(BulkImportPermissionTests.TestProxy));
            ((BulkImportPermissionTests.TestProxy)dependency).InvokeMethod = (method, _) =>
                throw new AssertionException("Unexpected dependency: " + method.Name);
            return dependency;
        }).ToArray();
        using var real = (global::UnitOfWork)constructor.Invoke(dependencies);
        var commitsReached = 0;
        var unitOfWork = Proxy<IUnitOfWork>((method, args) =>
        {
            if (method.Name == "CommitTransactionAsync")
            {
                commitsReached++;
                return Task.CompletedTask;
            }
            if (method.Name is "BeginTransactionAsync" or "RollbackTransactionAsync")
            {
                return Task.CompletedTask;
            }
            return method.Invoke(real, args);
        });
        var validation = new CommonDecodedResult
        {
            Success = true,
            UserEmployeeId = template.Id,
            LoggedInEmployeeId = template.Id,
            TenantId = template.TenantId!.Value,
            Claims = new TokenClaimsModel { TenantEncriptionKey = "probe-only" }
        };
        var common = Proxy<ICommonRequestService>((_, _) => Task.FromResult(validation));
        var permission = Proxy<IPermissionService>((method, _) => throw new AssertionException(method.Name));
        var create = new CreateBaseEmployeeInfoCommandHandler(
            unitOfWork, mapper, NullLogger<CreateBaseEmployeeInfoCommandHandler>.Instance,
            common, permission, encoder,
            Proxy<ITokenService>((_, _) => Task.FromResult("not-a-real-token")),
            Proxy<IEmailService>((_, _) => Task.FromResult(true)), config);
        var email = "contact-probe-" + Guid.NewGuid().ToString("N") + "@example.test";
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var response = await create.Handle(new CreateBaseEmployeeInfoCommand(new CreateBaseEmployeeRequestDTO
            {
                FirstName = "Contact", MiddleName = "Rollback", LastName = "Probe",
                OfficialEmail = email, CountryId = template.CountryId,
                DepartmentId = template.DepartmentId!.Value, DesignationId = template.DesignationId!.Value,
                EmployeeTypeId = template.EmployeeTypeId!.Value, GenderId = template.GenderId ?? 1,
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RoleId = roleId, HasPermanent = true, IsActive = true
            }), default);
            Assert.That(response.IsSucceeded, Is.True);
            var employee = await db.Employees.SingleAsync(item => item.OfficialEmail == email);
            var initial = await db.EmployeeContacts.SingleAsync(item => item.EmployeeId == employee.Id);
            Assert.Multiple(() =>
            {
                Assert.That(initial.ContactName, Is.EqualTo("Contact Rollback Probe"));
                Assert.That(initial.CountryId, Is.EqualTo(template.CountryId));
                Assert.That(initial.Relation, Is.Null);
                Assert.That(initial.ContactType, Is.Null);
                Assert.That(initial.ContactNumber, Is.Null);
                Assert.That(initial.LocalityId, Is.Null);
                Assert.That(initial.IsEditAllowed, Is.True);
            });
            validation.UserEmployeeId = employee.Id;
            validation.LoggedInEmployeeId = employee.Id;
            var locality = await db.Localities.AsNoTracking().FirstAsync(item => item.State.CountryId == template.CountryId);
            var update = new UpdateContactInfoCommandHandler(unitOfWork, NullLogger<UpdateContactInfoCommandHandler>.Instance, common);
            var updated = await update.Handle(new UpdateEmployeeContactCommand(new UpdateContactRequestDTO
            {
                Id = initial.Id, ContactName = "Owner Probe", Relation = (int)EmergencyContactRelation.Owner,
                ContactType = 2, ContactNumber = "+971501234567", AlternateNumber = "+971501234568",
                CountryId = template.CountryId, StateId = locality.StateId, DistrictId = locality.DistrictId,
                LocalityId = locality.Id, HouseNo = "TEST", Address = "Synthetic rollback address"
            }), default);
            Assert.That(updated.IsSucceeded, Is.True);
            var add = new CreateContactInfoCommandHandler(unitOfWork, mapper,
                NullLogger<CreateContactInfoCommandHandler>.Instance, permission, config, encoder, common);
            var added = await add.Handle(new CreateContactInfoCommand(new CreateContactRequestDTO
            {
                EmployeeId = employee.Id.ToString(),
                ContactName = "Second Probe", ContactNumber = "+971501234569",
                Relation = (int)EmergencyContactRelation.Other,
                ContactType = (axionpro.application.Constants.ConstantValues.ContactTypeEnum)1,
                CountryId = template.CountryId, StateId = locality.StateId, DistrictId = locality.DistrictId,
                LocalityId = locality.Id, IsPrimary = true
            }), default);
            Assert.That(added.IsSucceeded, Is.True);
            var readback = await real.EmployeeContactRepository.GetInfo(employee.Id,
                new GetContactRequestDTO { EmployeeId = employee.Id.ToString(), UserEmployeeId = employee.Id.ToString() });
            Assert.That(readback.Data, Has.Count.EqualTo(2));
            var edited = readback.Data.Single(item => item.Id == initial.Id);
            Assert.Multiple(() =>
            {
                Assert.That(edited.Relation, Is.EqualTo((int)EmergencyContactRelation.Owner));
                Assert.That(edited.ContactNumber, Is.EqualTo("+971501234567"));
                Assert.That(edited.LocalityId, Is.EqualTo(locality.Id));
                Assert.That(edited.LocalityName, Is.EqualTo(locality.LocalityName));
                Assert.That(commitsReached, Is.EqualTo(3));
            });
        }
        finally
        {
            await transaction.RollbackAsync();
            db.ChangeTracker.Clear();
        }
        Assert.That(await db.Employees.CountAsync(), Is.EqualTo(initialEmployeeCount));
        Assert.That(await db.EmployeeContacts.CountAsync(), Is.EqualTo(initialContactCount));
        Assert.That(await db.LoginCredentials.AnyAsync(item => item.LoginId == email), Is.False);
    }

    private static IConfiguration Configuration()
    {
        var path = Environment.GetEnvironmentVariable("AXIONPRO_CONTACT_DB_SETTINGS");
        if (string.IsNullOrWhiteSpace(path))
        {
            Assert.Ignore("Opt-in local EmployeeContact database probe; schema and UAE seed required.");
        }
        return new ConfigurationBuilder().AddJsonFile(path!).Build();
    }

    private static WorkforceDbContext Database(IConfiguration config) => new(
        new DbContextOptionsBuilder<WorkforceDbContext>().UseNpgsql(config.GetConnectionString("DefaultConnection")).Options);

    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> invoke) where T : class
    {
        var proxy = DispatchProxy.Create<T, BulkImportPermissionTests.TestProxy>();
        ((BulkImportPermissionTests.TestProxy)(object)proxy).InvokeMethod = invoke;
        return proxy;
    }
}
