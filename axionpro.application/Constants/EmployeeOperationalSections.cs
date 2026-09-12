namespace axionpro.application.Constants;

/// <summary>Existing Employee leaf codes which share tenant self-service defaults.</summary>
public static class EmployeeOperationalSections
{
    public static IReadOnlyList<string> ModuleCodes { get; } = Array.AsReadOnly(new[]
    {
        "EMP_WORK_LOCATIONS", "EMP_DEVICES", "EMP_WORK_ARRANGEMENT",
        "EMP_WORK_PATTERN", "EMP_OVERRIDES"
    });
}
