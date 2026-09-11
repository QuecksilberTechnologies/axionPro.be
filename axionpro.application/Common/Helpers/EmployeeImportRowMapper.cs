using System.ComponentModel.DataAnnotations;
using System.Globalization;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;

namespace axionpro.application.Common.Helpers;

/// <summary>Converts explicit import columns into existing Employee and Contact request DTOs.</summary>
public static class EmployeeImportRowMapper
{
    #region Row Conversion

    public static (CreateBaseEmployeeRequestDTO Employee, CreateContactRequestDTO? Contact) Map(
        BulkImportPreviewRowDTO row, bool excel = false, bool date1904 = false)
    {
        string Text(string field) => row.Values.GetValueOrDefault(field, string.Empty);
        int Number(string field, bool required = true)
        {
            var value = Text(field);
            if (!required && string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }
            if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) || number <= 0)
            {
                row.Errors.Add($"{field} must be an existing positive identifier.");
            }
            return number;
        }
        bool Flag(string field)
        {
            if (!bool.TryParse(Text(field), out var value))
            {
                row.Errors.Add($"{field} must be true or false.");
            }
            return value;
        }
        DateTime? Date(string field)
        {
            var value = ParseDate(Text(field), excel, date1904);
            if (value is null)
            {
                row.Errors.Add($"{field} is required as an unambiguous yyyy-MM-dd date.");
            }
            else
            {
                // Save canonical dates in the draft; the worker never reinterprets Excel serials.
                row.Values[field] = value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            return value;
        }
        var employee = new CreateBaseEmployeeRequestDTO
        {
            FirstName = Text(nameof(CreateBaseEmployeeRequestDTO.FirstName)),
            LastName = Text(nameof(CreateBaseEmployeeRequestDTO.LastName)),
            MiddleName = Text(nameof(CreateBaseEmployeeRequestDTO.MiddleName)),
            OfficialEmail = Text(nameof(CreateBaseEmployeeRequestDTO.OfficialEmail)),
            DateOfBirth = Date(nameof(CreateBaseEmployeeRequestDTO.DateOfBirth)),
            DateOfOnBoarding = Date(nameof(CreateBaseEmployeeRequestDTO.DateOfOnBoarding)),
            GenderId = Number(nameof(CreateBaseEmployeeRequestDTO.GenderId)),
            CountryId = Number(nameof(CreateBaseEmployeeRequestDTO.CountryId)),
            DepartmentId = Number(nameof(CreateBaseEmployeeRequestDTO.DepartmentId)),
            DesignationId = Number(nameof(CreateBaseEmployeeRequestDTO.DesignationId)),
            EmployeeTypeId = Number(nameof(CreateBaseEmployeeRequestDTO.EmployeeTypeId)),
            RoleId = Number(nameof(CreateBaseEmployeeRequestDTO.RoleId), false),
            HasPermanent = Flag(nameof(CreateBaseEmployeeRequestDTO.HasPermanent)),
            IsActive = Flag(nameof(CreateBaseEmployeeRequestDTO.IsActive))
        };
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(employee, new ValidationContext(employee), results, true);
        row.Errors.AddRange(results.Select(result => result.ErrorMessage ?? "Invalid Employee field."));

        CreateContactRequestDTO? contact = null;
        if (BulkImportConstants.EmployeeColumns.Skip(16).Any(field => !string.IsNullOrWhiteSpace(Text(field))))
        {
            int? OptionalId(string field)
            {
                var value = Number(field, false);
                return value == 0 ? null : value;
            }
            contact = new CreateContactRequestDTO
            {
                ContactType = ConstantValues.ContactTypeEnum.Personal,
                IsPrimary = true,
                ContactName = Text(nameof(CreateContactRequestDTO.ContactName)),
                ContactNumber = Text(nameof(CreateContactRequestDTO.ContactNumber)),
                AlternateNumber = Text(nameof(CreateContactRequestDTO.AlternateNumber)),
                Email = Text("ContactEmail"),
                CountryId = OptionalId("ContactCountryId"),
                StateId = OptionalId(nameof(CreateContactRequestDTO.StateId)),
                DistrictId = OptionalId(nameof(CreateContactRequestDTO.DistrictId)),
                HouseNo = Text(nameof(CreateContactRequestDTO.HouseNo)),
                Street = Text(nameof(CreateContactRequestDTO.Street)),
                LandMark = Text(nameof(CreateContactRequestDTO.LandMark)),
                Address = Text(nameof(CreateContactRequestDTO.Address))
            };
            if (string.IsNullOrWhiteSpace(contact.ContactNumber))
            {
                row.Errors.Add("ContactNumber is required when contact/address information is supplied.");
            }
            if (!string.IsNullOrWhiteSpace(contact.Email) && !new EmailAddressAttribute().IsValid(contact.Email))
            {
                row.Errors.Add("ContactEmail must be a valid email address.");
            }
        }
        return (employee, contact);
    }

    #endregion

    #region Date Parsing

    public static DateTime? ParseDate(string value, bool excel, bool date1904)
    {
        if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date))
        {
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
        if (!excel || !double.TryParse(value, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var serial) || !double.IsFinite(serial) ||
            serial != Math.Truncate(serial) || serial < (date1904 ? 0 : 61))
        {
            return null;
        }
        try
        {
            return DateTime.SpecifyKind(DateTime.FromOADate(serial + (date1904 ? 1462 : 0)), DateTimeKind.Utc);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    #endregion
}
