// ================================================================
// Purpose : Defines command-specific MQTT protocol policy. Internal command
//           tracking is never added to device JSON payloads.
// ================================================================

using System.Text.Json;
using axionpro.application.Exceptions;
using axionpro.domain.Entity;

namespace axionpro.application.Constants;

/// <summary>Defines the protocol and authorization policy for one vendor command.</summary>
public sealed record DeviceProtocolCommandDefinition(
    string Name,
    DeviceCommandAccessLevel AccessLevel,
    DeviceCommandResponseMode ResponseMode,
    IReadOnlyList<string> ResponseMatchFields);

/// <summary>
/// Catalogues vendor commands confirmed in the MQTT Postman collection. Unknown
/// commands are rejected rather than being forwarded to a physical device.
/// </summary>
public static class DeviceProtocolCommandCatalog
{
    private static readonly IReadOnlyDictionary<string, DeviceProtocolCommandDefinition> Definitions =
        BuildDefinitions();

    /// <summary>Gets a known command policy or rejects an unconfirmed command.</summary>
    public static DeviceProtocolCommandDefinition GetRequired(string commandName)
    {
        var normalized = Normalize(commandName);
        return Definitions.TryGetValue(normalized, out var definition)
            ? definition
            : throw new ValidationErrorException("The requested device command is not part of the supported MQTT protocol catalog.");
    }

    /// <summary>Normalizes and validates a command payload without mutating vendor JSON.</summary>
    public static string ValidatePayload(string commandName, string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ValidationErrorException("A device command payload is required.");
        }

        using var document = JsonDocument.Parse(payload);
        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("cmd", out var commandProperty) ||
            commandProperty.ValueKind != JsonValueKind.String ||
            !string.Equals(Normalize(commandProperty.GetString()), Normalize(commandName), StringComparison.Ordinal))
        {
            throw new ValidationErrorException("The payload cmd must match the requested supported device command.");
        }

        foreach (var forbiddenField in new[] { "correlationid", "requestid", "commandid", "internaltrackingid" })
        {
            if (document.RootElement.EnumerateObject().Any(property =>
                    string.Equals(property.Name, forbiddenField, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationErrorException("Internal AxionPro tracking fields must not be sent in a device payload.");
            }
        }

        return document.RootElement.GetRawText();
    }

    /// <summary>Builds persisted protocol matching criteria from documented request/response fields only.</summary>
    public static string BuildMatchCriteria(DeviceProtocolCommandDefinition definition, string payload)
    {
        using var document = JsonDocument.Parse(payload);
        var criteria = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in definition.ResponseMatchFields)
        {
            if (document.RootElement.TryGetProperty(field, out var value))
            {
                criteria[field] = value.Clone();
            }
        }

        return JsonSerializer.Serialize(criteria);
    }

    /// <summary>Tests a response against the documented fields retained on a command.</summary>
    public static bool ResponseMatches(DeviceCommand command, JsonElement response)
    {
        if (!response.TryGetProperty("ret", out var ret) || ret.ValueKind != JsonValueKind.String ||
            !string.Equals(Normalize(ret.GetString()), command.CommandName, StringComparison.Ordinal))
        {
            return false;
        }

        using var criteriaDocument = JsonDocument.Parse(command.MatchCriteria);
        foreach (var property in criteriaDocument.RootElement.EnumerateObject())
        {
            if (!response.TryGetProperty(property.Name, out var responseValue) ||
                !JsonElement.DeepEquals(property.Value, responseValue))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyDictionary<string, DeviceProtocolCommandDefinition> BuildDefinitions()
    {
        var allVendorCommands = typeof(DeviceCommands)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(field => field.FieldType == typeof(string))
            .Select(field => (string?)field.GetRawConstantValue())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Normalize(value))
            .Where(value => value is not "reg" and not "sendlog" and not "senduser")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                value => value,
                value => new DeviceProtocolCommandDefinition(
                    value,
                    DeviceCommandAccessLevel.HostOnly,
                    DeviceCommandResponseMode.Required,
                    Array.Empty<string>()),
                StringComparer.OrdinalIgnoreCase);

        ConfigureTenant(allVendorCommands, DeviceCommands.SetUserInfo, "enrollid");
        ConfigureTenant(allVendorCommands, DeviceCommands.AddUser, "enrollid");
        ConfigureTenant(allVendorCommands, DeviceCommands.DeleteUser);
        ConfigureTenant(allVendorCommands, DeviceCommands.EnableUser);
        ConfigureTenant(allVendorCommands, DeviceCommands.GetDeviceCapabilities);
        ConfigureTenant(allVendorCommands, DeviceCommands.GetDeviceInfo);
        ConfigureTenant(allVendorCommands, DeviceCommands.GetTime);
        ConfigureTenant(allVendorCommands, DeviceCommands.SetTime);
        ConfigureTenant(allVendorCommands, DeviceCommands.GetUserInfo, "enrollid", "backupnum");
        ConfigureTenant(
            allVendorCommands,
            DeviceCommands.Reboot,
            DeviceCommandAccessLevel.TenantPermission,
            DeviceCommandResponseMode.PublishOnly);
        ConfigureTenant(allVendorCommands, DeviceCommands.GetDoorStatus, accessLevel: DeviceCommandAccessLevel.TenantAccessControlPermission);
        ConfigureTenant(allVendorCommands, DeviceCommands.OpenDoor, accessLevel: DeviceCommandAccessLevel.TenantAccessControlPermission);
        ConfigureTenant(allVendorCommands, DeviceCommands.LockControl, accessLevel: DeviceCommandAccessLevel.TenantAccessControlPermission);

        ConfigureStream(allVendorCommands, DeviceCommands.GetAllLog);
        ConfigureStream(allVendorCommands, DeviceCommands.GetAllUsers);
        ConfigureStream(allVendorCommands, DeviceCommands.GetNewLog);
        ConfigureStream(allVendorCommands, DeviceCommands.GetUserList);

        return allVendorCommands;
    }

    private static void ConfigureTenant(
        IDictionary<string, DeviceProtocolCommandDefinition> definitions,
        string command,
        params string[] responseMatchFields) =>
        ConfigureTenant(definitions, command, DeviceCommandAccessLevel.TenantPermission, DeviceCommandResponseMode.Required, responseMatchFields);

    private static void ConfigureTenant(
        IDictionary<string, DeviceProtocolCommandDefinition> definitions,
        string command,
        DeviceCommandAccessLevel accessLevel,
        DeviceCommandResponseMode responseMode = DeviceCommandResponseMode.Required,
        params string[] responseMatchFields)
    {
        var normalized = Normalize(command);
        definitions[normalized] = new DeviceProtocolCommandDefinition(normalized, accessLevel, responseMode, responseMatchFields);
    }

    private static void ConfigureStream(IDictionary<string, DeviceProtocolCommandDefinition> definitions, string command)
    {
        var normalized = Normalize(command);
        definitions[normalized] = new DeviceProtocolCommandDefinition(
            normalized,
            DeviceCommandAccessLevel.HostOnly,
            DeviceCommandResponseMode.Streamed,
            Array.Empty<string>());
    }

    private static string Normalize(string? value) => value?.Trim().ToLowerInvariant() ?? string.Empty;
}
