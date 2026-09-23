using System.Globalization;
using System.Text;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.Holiday;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.HolidayCmd;

#region Import

public sealed class ImportHolidaysCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<ImportHolidaysCommand, ApiResponse<HolidayImportResultDTO>>
{
    private static readonly string[] RequiredColumns =
    [
        "TenantLocationId", "HolidayName", "HolidayDate", "IsOptional", "Description", "Icon"
    ];

    public async Task<ApiResponse<HolidayImportResultDTO>> Handle(
        ImportHolidaysCommand request,
        CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        var table = await BulkImportTableReader.ReadAsync(request.DTO, cancellationToken);
        if (table.Columns.Select(name => name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count()
            != table.Columns.Count)
        {
            throw new ValidationErrorException("Holiday import has duplicate column headers.");
        }

        var positions = table.Columns
            .Select((name, index) => (name, index))
            .ToDictionary(pair => pair.name.Trim(), pair => pair.index, StringComparer.OrdinalIgnoreCase);
        var missing = RequiredColumns.Where(name => !positions.ContainsKey(name)).ToList();
        if (missing.Count > 0)
        {
            throw new ValidationErrorException("Required holiday columns are missing: " + string.Join(", ", missing));
        }

        if (table.Rows.Count == 0)
        {
            throw new ValidationErrorException("The holiday import has no data rows.");
        }

        var errors = new List<string>();
        var parsed = new List<Holiday>();
        var fileKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var rowNumbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in table.Rows)
        {
            var locationText = Value(row.Values, positions, "TenantLocationId");
            var name = Value(row.Values, positions, "HolidayName").Trim();
            var dateText = Value(row.Values, positions, "HolidayDate");
            var optionalText = Value(row.Values, positions, "IsOptional");
            var description = Value(row.Values, positions, "Description").Trim();
            var icon = Value(row.Values, positions, "Icon").Trim();

            if (!long.TryParse(locationText, NumberStyles.None, CultureInfo.InvariantCulture, out var locationId)
                || locationId <= 0
                || !DateOnly.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var holidayDate)
                || !bool.TryParse(optionalText, out var isOptional)
                || string.IsNullOrWhiteSpace(name)
                || name.Length > 100
                || description.Length > 255
                || icon.Length > 100)
            {
                errors.Add($"Row {row.RowNumber}: use a valid TenantLocationId, HolidayName, ISO HolidayDate, boolean IsOptional, Description up to 255 characters and Icon up to 100 characters.");
                continue;
            }

            var key = Key(locationId, holidayDate);
            if (!fileKeys.Add(key))
            {
                errors.Add($"Row {row.RowNumber}: another holiday already uses this location and date in the uploaded file.");
                continue;
            }

            rowNumbers[key] = row.RowNumber;

            parsed.Add(new Holiday
            {
                TenantId = actor.TenantId,
                TenantLocationId = locationId,
                HolidayName = name,
                HolidayDate = holidayDate,
                IsOptional = isOptional,
                Description = description.Length == 0 ? null : description,
                Icon = icon.Length == 0 ? null : icon,
                IsActive = true,
                IsSoftDeleted = false,
                AddedById = actor.LoggedInEmployeeId,
                AddedDateTime = DateTime.UtcNow
            });
        }

        foreach (var locationId in parsed.Select(item => item.TenantLocationId).Distinct())
        {
            if (!await unitOfWork.HolidayRepository.TenantLocationExistsAsync(
                actor.TenantId, locationId, cancellationToken))
            {
                errors.Add($"TenantLocationId {locationId} is inactive or does not belong to this tenant.");
            }
        }

        if (errors.Count == 0)
        {
            var existing = await unitOfWork.HolidayRepository.GetTenantHolidaysForDuplicateCheckAsync(
                actor.TenantId, cancellationToken);
            var existingByDate = existing
                .GroupBy(item => Key(item.TenantLocationId, item.HolidayDate), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
            foreach (var item in parsed)
            {
                if (existingByDate.TryGetValue(Key(item.TenantLocationId, item.HolidayDate), out var conflict))
                {
                    var rowNumber = rowNumbers[Key(item.TenantLocationId, item.HolidayDate)];
                    errors.Add($"Row {rowNumber}: {HolidayValidation.DuplicateMessage(conflict)}");
                }
            }
        }

        if (errors.Count > 0)
        {
            throw new ValidationErrorException("Holiday import validation failed; no rows were saved.", errors);
        }

        var created = await unitOfWork.HolidayRepository.ImportHolidaysAsync(parsed, cancellationToken);

        return ApiResponse<HolidayImportResultDTO>.Success(new HolidayImportResultDTO
        {
            TotalRows = table.Rows.Count,
            CreatedCount = created,
            SkippedExistingCount = 0
        }, "Holiday import completed successfully.");
    }

    private static string Value(IReadOnlyList<string> values, IReadOnlyDictionary<string, int> positions, string name)
    {
        var index = positions[name];
        return index < values.Count ? values[index] : string.Empty;
    }

    private static string Key(long locationId, DateOnly date)
    {
        return string.Join("|", locationId.ToString(CultureInfo.InvariantCulture),
            date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}

#endregion

#region Export

public sealed class ExportHolidaysQueryHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<ExportHolidaysQuery, byte[]>
{
    public async Task<byte[]> Handle(ExportHolidaysQuery request, CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (request.DTO.TenantLocationId <= 0 || request.DTO.HolidayYear is < 1 or > 9999)
        {
            throw new ValidationErrorException("TenantLocationId and HolidayYear must be valid when supplied.");
        }

        var holidays = await unitOfWork.HolidayRepository.GetTenantHolidaysAsync(
            actor.TenantId,
            request.DTO.TenantLocationId,
            request.DTO.HolidayYear,
            cancellationToken);
        var csv = new StringBuilder("TenantLocationId,HolidayName,HolidayDate,IsOptional,Description,Icon\r\n");
        foreach (var holiday in holidays)
        {
            csv.Append(holiday.TenantLocationId.ToString(CultureInfo.InvariantCulture)).Append(',')
                .Append(Escape(holiday.HolidayName)).Append(',')
                .Append(holiday.HolidayDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).Append(',')
                .Append(holiday.IsOptional ? "true" : "false").Append(',')
                .Append(Escape(holiday.Description ?? string.Empty)).Append(',')
                .Append(Escape(holiday.Icon ?? string.Empty)).Append("\r\n");
        }

        return new UTF8Encoding(true).GetBytes(csv.ToString());
    }

    private static string Escape(string value)
    {
        var safe = value.Length > 0 && "=+-@".Contains(value[0]) ? "'" + value : value;
        return '"' + safe.Replace("\"", "\"\"") + '"';
    }
}

#endregion
