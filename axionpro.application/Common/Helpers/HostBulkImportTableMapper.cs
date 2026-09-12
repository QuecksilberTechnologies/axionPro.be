using System.Globalization;
using System.Reflection;
using System.Text.Json;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;

namespace axionpro.application.Common.Helpers;

/// <summary>
/// Maps explicit spreadsheet headers to the existing Host catalogue contracts.
/// Permission, Tenant and audit identifiers never come from spreadsheet cells.
/// </summary>
public static class HostBulkImportTableMapper
{
    #region Column contracts

    public static IReadOnlyList<string> DeviceColumns => Columns(typeof(DeviceMasterRequestDTO));
    public static IReadOnlyList<string> CardColumns => Columns(typeof(TenantCardMasterRequestDTO));

    private static IReadOnlyList<string> Columns(Type contract)
    {
        return contract.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(property => property.CanWrite)
            .Select(property => property.Name)
            .ToArray();
    }

    #endregion

    #region Explicit mapping

    public static Dictionary<string, string> ResolveColumns(
        IReadOnlyList<string> sourceColumns,
        IReadOnlyList<string> targetColumns,
        string? mappingJson)
    {
        var protectedColumns = new[]
        {
            "TenantId", "ModuleId", "OperationId", "AddedById", "UpdatedById",
            "HostUserId", "Id", "IsOccupied", "TenantDeviceId", "TenantLocationId"
        };
        if (sourceColumns.Any(column => protectedColumns.Contains(column, StringComparer.OrdinalIgnoreCase)))
        {
            throw new ValidationErrorException("Tenant, permission, assignment and audit fields cannot be imported from spreadsheet columns.");
        }
        if (sourceColumns.Any(string.IsNullOrWhiteSpace) ||
            sourceColumns.Distinct(StringComparer.OrdinalIgnoreCase).Count() != sourceColumns.Count)
        {
            throw new ValidationErrorException("Column headers must be non-empty and unique.");
        }

        Dictionary<string, string> requested;
        try
        {
            requested = string.IsNullOrWhiteSpace(mappingJson)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(mappingJson)
                    ?? throw new JsonException();
        }
        catch (JsonException)
        {
            throw new ValidationErrorException("ColumnMappingJson must be a target-to-source JSON object.");
        }

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in requested)
        {
            var target = targetColumns.SingleOrDefault(column =>
                string.Equals(column, pair.Key, StringComparison.OrdinalIgnoreCase));
            var source = sourceColumns.SingleOrDefault(column =>
                string.Equals(column, pair.Value, StringComparison.OrdinalIgnoreCase));
            if (target is null || source is null || !result.TryAdd(target, source))
            {
                throw new ValidationErrorException("Column mapping contains an unknown or repeated field.");
            }
        }

        foreach (var target in targetColumns)
        {
            var source = sourceColumns.SingleOrDefault(column =>
                string.Equals(column, target, StringComparison.OrdinalIgnoreCase));
            if (source is not null)
            {
                result.TryAdd(target, source);
            }
        }

        if (result.Values.Distinct(StringComparer.OrdinalIgnoreCase).Count() != result.Count)
        {
            throw new ValidationErrorException("One source column cannot populate multiple target fields.");
        }

        return result;
    }

    public static T ReadRow<T>(
        BulkImportTableDTO table,
        BulkImportSourceRowDTO row,
        IReadOnlyDictionary<string, string> mapping) where T : new()
    {
        var contract = typeof(T) == typeof(CreateDeviceMasterRequestDTO)
            ? typeof(DeviceMasterRequestDTO)
            : typeof(T) == typeof(CreateTenantCardMasterRequestDTO)
                ? typeof(TenantCardMasterRequestDTO)
                : throw new ArgumentException("Unsupported Host bulk contract.");
        var allowed = contract.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var dto = new T();
        foreach (var entry in mapping)
        {
            var property = allowed.SingleOrDefault(candidate => candidate.Name == entry.Key)
                ?? throw new ValidationErrorException("Spreadsheet contains a field outside the import contract.");
            var index = table.Columns.FindIndex(column => string.Equals(column, entry.Value, StringComparison.OrdinalIgnoreCase));
            if (index < 0 || index >= row.Values.Count)
            {
                throw new ValidationErrorException($"Row {row.RowNumber}: mapped column is missing.");
            }

            var value = row.Values[index].Trim();
            if (value.Length == 0)
            {
                continue;
            }

            try
            {
                property.SetValue(dto, Parse(property.PropertyType, value));
            }
            catch (Exception error) when (error is FormatException or OverflowException or ArgumentException)
            {
                // Do not echo cell contents: card numbers and other private values stay out of errors.
                throw new ValidationErrorException($"Row {row.RowNumber}: invalid format for {property.Name}.");
            }
        }

        return dto;
    }

    #endregion

    #region Typed cell conversion

    private static object Parse(Type declaredType, string value)
    {
        var type = Nullable.GetUnderlyingType(declaredType) ?? declaredType;
        if (type == typeof(string)) return value;
        if (type == typeof(bool)) return bool.Parse(value);
        if (type == typeof(int)) return int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        if (type == typeof(decimal)) return decimal.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        if (type == typeof(DateOnly)) return DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (type.IsEnum)
        {
            var parsed = Enum.Parse(type, value, true);
            return Enum.IsDefined(type, parsed) ? parsed : throw new FormatException();
        }

        throw new ArgumentException("Unsupported import field type.");
    }

    #endregion
}
