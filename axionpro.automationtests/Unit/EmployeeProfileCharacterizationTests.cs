// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Locks the current Employee profile percentage and API
//           contracts before any common-policy refactoring.
// ================================================================

using System.Reflection;
using axionpro.api.Controllers.Employee;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Extentions;
using axionpro.application.Features.EmployeeCmd;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

/// <summary>
/// Characterizes current Employee profile behavior. These assertions intentionally
/// describe the existing output and must be reviewed before changing a formula or contract.
/// </summary>
[TestFixture]
[Category("EmployeeProfileCharacterization")]
public sealed class EmployeeProfileCharacterizationTests
{
    #region Percentage behavior

    /// <summary>
    /// Verifies an absent section remains a non-created, zero-percent, locked section.
    /// </summary>
    [Test]
    public void Empty_supported_sections_keep_the_current_zero_percent_contract()
    {
        var education = Array.Empty<GetEducationResponseDTO>().CalculateEducationCompletionDTO();
        var bank = Array.Empty<GetBankResponseDTO>().CalculateBankCompletionDTO();
        var contact = Array.Empty<GetContactResponseDTO>().CalculateContactCompletionDTO();

        Assert.Multiple(() =>
        {
            AssertEmpty(education, "Education");
            AssertEmpty(bank, "Bank");
            AssertEmpty(contact, "Contact");
        });
    }

    /// <summary>
    /// Locks the current six-field Education formula and first-row status behavior.
    /// </summary>
    [Test]
    public void Education_uses_six_equal_checks_and_first_row_status()
    {
        var result = new[]
        {
            new GetEducationResponseDTO
            {
                Degree = "BSc",
                InstituteName = "Example University",
                StartDate = new DateOnly(2020, 1, 1),
                EndDate = new DateOnly(2023, 1, 1),
                HasEducationDocUploded = true,
                ScoreType = "Percentage",
                IsInfoVerified = true,
                IsEditAllowed = false
            }
        }.CalculateEducationCompletionDTO();

        Assert.Multiple(() =>
        {
            Assert.That(result.CompletionPercent, Is.EqualTo(100));
            Assert.That(result.IsInfoVerified, Is.True);
            Assert.That(result.IsEditAllowed, Is.False);
            Assert.That(result.IsSectionCreate, Is.True);
        });
    }

    /// <summary>
    /// Locks the current primary Bank requirement: otherwise a complete row is capped at 99 percent.
    /// </summary>
    [Test]
    public void Bank_without_a_primary_account_is_capped_at_ninety_nine_percent()
    {
        var result = new[]
        {
            new GetBankResponseDTO
            {
                BankName = "Example Bank",
                BranchName = "Main",
                IFSCCode = "TEST0001",
                AccountNumber = "1234",
                AccountType = "Savings",
                IsPrimaryAccount = false,
                IsInfoVerified = true,
                IsEditAllowed = true
            }
        }.CalculateBankCompletionDTO();

        Assert.Multiple(() =>
        {
            Assert.That(result.CompletionPercent, Is.EqualTo(99));
            Assert.That(result.IsInfoVerified, Is.True);
            Assert.That(result.IsEditAllowed, Is.False,
                "The approved verification lock overrides stale stored edit flags without changing completion.");
        });
    }

    /// <summary>
    /// Locks the current multi-row status aggregation: all verified and any editable.
    /// </summary>
    [Test]
    public void Contact_aggregates_all_verified_and_any_editable_across_rows()
    {
        var result = new[]
        {
            new GetContactResponseDTO
            {
                Id = 1,
                ContactName = "One",
                ContactNumber = "100",
                Relation = 1,
                IsPrimary = true,
                IsInfoVerified = true,
                IsEditAllowed = false
            },
            new GetContactResponseDTO
            {
                Id = 2,
                ContactName = "Two",
                ContactNumber = "200",
                Relation = 2,
                IsPrimary = false,
                IsInfoVerified = false,
                IsEditAllowed = true
            }
        }.CalculateContactCompletionDTO();

        Assert.Multiple(() =>
        {
            Assert.That(result.CompletionPercent, Is.EqualTo(100));
            Assert.That(result.IsInfoVerified, Is.False);
            Assert.That(result.IsEditAllowed, Is.True);
        });
    }

    /// <summary>
    /// Verifies the common calculator excludes workflow flags from data completion.
    /// </summary>
    [Test]
    public void Common_calculator_uses_only_required_data_fields()
    {
        var percentage = EmployeeProfileCompletionCalculator.CalculateRowPercentage(
            true,
            false,
            true,
            true);

        Assert.That(percentage, Is.EqualTo(75));
    }

    /// <summary>
    /// Verifies verification always overrides editability in the common section result.
    /// </summary>
    [Test]
    public void Verified_section_is_never_reported_as_editable()
    {
        var result = EmployeeProfileCompletionCalculator.CreateSection(
            "Identity",
            new[] { 100d },
            new bool?[] { true },
            new bool?[] { true });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsInfoVerified, Is.True);
            Assert.That(result.IsEditAllowed, Is.False);
        });
    }

    #endregion

    #region Permission pipeline coverage

    /// <summary>
    /// Verifies every concrete EmployeeCmd request resolves to a server-owned leaf module and carries
    /// a permission DTO through its request, DTO, Filter, or PermissionRequest contract.
    /// </summary>
    [Test]
    public void Every_employee_request_is_mapped_to_a_leaf_module_and_carries_permission_identifiers()
    {
        var requestTypes = typeof(EmployeeTenantPermissionBehavior<,>).Assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.Namespace?.StartsWith(
                "axionpro.application.Features.EmployeeCmd",
                StringComparison.Ordinal) == true)
            .Where(type => type.GetInterfaces().Any(contract =>
                contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IRequest<>)))
            .OrderBy(type => type.FullName)
            .ToArray();

        var failures = new List<string>();
        foreach (var requestType in requestTypes)
        {
            var behaviorType = typeof(EmployeeTenantPermissionBehavior<,>)
                .MakeGenericType(requestType, typeof(object));
            var resolver = behaviorType.GetMethod(
                "ResolveExpectedModuleCode",
                BindingFlags.NonPublic | BindingFlags.Static);
            var moduleCode = resolver?.Invoke(null, null) as string;
            var carriesPermission = CarriesPermissionRequest(requestType);

            if (string.IsNullOrWhiteSpace(moduleCode) || !carriesPermission)
            {
                failures.Add($"{requestType.FullName}: module={moduleCode ?? "<none>"}, permissionDTO={carriesPermission}");
            }
        }

        Assert.That(requestTypes, Is.Not.Empty);
        Assert.That(failures, Is.Empty,
            "Employee permission pipeline gaps:\n" + string.Join("\n", failures));
    }

    #endregion

    #region API and DTO contracts

    /// <summary>
    /// Verifies the profile-status endpoint remains an authenticated GET with its current route.
    /// </summary>
    [Test]
    public void Get_all_percentage_route_and_response_envelope_remain_explicit()
    {
        var action = typeof(EmployeeController).GetMethod(
            nameof(EmployeeController.GetAllEmployeePercentageAsync));
        var route = action?.GetCustomAttributes<HttpMethodAttribute>().Single();
        var returnContract = typeof(ApiResponse<List<CompletionSectionDTO>>);

        Assert.Multiple(() =>
        {
            Assert.That(route?.HttpMethods, Does.Contain("GET"));
            Assert.That(route?.Template, Is.EqualTo("get-all-percentage"));
            Assert.That(action?.GetCustomAttributes().Any(attribute =>
                attribute.GetType().Name == "AuthorizeAttribute"), Is.True);
            Assert.That(returnContract.GetProperty(nameof(ApiResponse<object>.Data)), Is.Not.Null);
            Assert.That(returnContract.GetProperty("Sections"), Is.Null,
                "The backend currently returns the section list in ApiResponse.Data.");
        });
    }

    /// <summary>
    /// Verifies the current shared section enum has only the original eight sections.
    /// </summary>
    [Test]
    public void Tab_info_type_keeps_the_current_eight_section_contract()
    {
        Assert.That(Enum.GetNames<TabInfoType>(), Is.EqualTo(new[]
        {
            "Employee", "Bank", "Contact", "Experience", "Identity", "Education", "Dependent", "Insurance"
        }));
    }

    [Test]
    public void Profile_status_exposes_update_bulk_ids_only_for_persisted_verification_sections()
    {
        var names = new[]
        {
            "Overview", "Bank", "Contact", "Experience", "Insurance", "Identity", "Education",
            "Dependent", "Work Locations", "Devices", "Work Arrangement", "Work Pattern", "Overrides"
        };
        var sections = EmployeeProfileCompletionCalculator.ApplyVerificationContract(
            names.Select(name => new CompletionSectionDTO { SectionName = name }).ToList());

        Assert.Multiple(() =>
        {
            Assert.That(sections.Where(section => section.CanUpdateVerificationStatus)
                .Select(section => section.TabInfoType), Is.EqualTo(new int?[] { 1, 2, 3, 4, 5, 6, 7 }));
            Assert.That(sections.Where(section => !section.CanUpdateVerificationStatus)
                .Select(section => section.SectionName), Is.EqualTo(new[]
                { "Insurance", "Work Locations", "Devices", "Work Arrangement", "Work Pattern", "Overrides" }));
            Assert.That(sections.Where(section => !section.CanUpdateVerificationStatus)
                .All(section => section.TabInfoType is null), Is.True);
        });
    }

    /// <summary>
    /// Verifies each legacy Employee profile controller keeps its currently published CRUD routes.
    /// </summary>
    [TestCase(typeof(BankController), "POST:create", "GET:get", "POST:update", "DELETE:delete")]
    [TestCase(typeof(ContactController), "POST:create", "GET:get", "POST:update", "DELETE:delete")]
    [TestCase(typeof(EducationController), "POST:create", "GET:get", "POST:update-education", "DELETE:delete")]
    [TestCase(typeof(ExperienceController), "POST:create", "GET:get", "POST:update", "DELETE:delete", "DELETE:delete-doc")]
    [TestCase(typeof(DependentController), "POST:create", "GET:get", "GET:get-in-detail", "POST:update", "DELETE:delete")]
    [TestCase(typeof(InsuranceController), "POST:employee-insurance-enroll", "GET:get-all-enroll", "DELETE:delete")]
    [TestCase(typeof(SensitiveController), "POST:Create", "GET:get")]
    public void Legacy_profile_controller_keeps_its_current_route_surface(
        Type controllerType,
        params string[] expectedRoutes)
    {
        var actualRoutes = controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .SelectMany(method => method.GetCustomAttributes<HttpMethodAttribute>())
            .Select(attribute => $"{attribute.HttpMethods.Single()}:{attribute.Template}")
            .ToArray();

        Assert.That(actualRoutes, Is.EquivalentTo(expectedRoutes));
    }

    #endregion

    #region Test helpers

    private static void AssertEmpty(CompletionSectionDTO section, string expectedName)
    {
        Assert.Multiple(() =>
        {
            Assert.That(section.SectionName, Is.EqualTo(expectedName));
            Assert.That(section.CompletionPercent, Is.EqualTo(0));
            Assert.That(section.IsInfoVerified, Is.False);
            Assert.That(section.IsEditAllowed, Is.False);
            Assert.That(section.IsSectionCreate, Is.False);
        });
    }

    private static bool CarriesPermissionRequest(Type requestType)
    {
        foreach (var propertyName in new[] { "DTO", "Filter", "PermissionRequest" })
        {
            var property = requestType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property is not null && typeof(PermissionRequestDTO).IsAssignableFrom(property.PropertyType))
            {
                return true;
            }

            var field = requestType.GetField(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (field is not null && typeof(PermissionRequestDTO).IsAssignableFrom(field.FieldType))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
