namespace axionpro.application.Constants;

/// <summary>Central parsing limits and column contracts for master import previews.</summary>
public static class BulkImportConstants
{
    public const int MaxFileBytes = 5 * 1024 * 1024;
    public const int MaxExpandedBytes = 25 * 1024 * 1024;
    public const int MaxRows = 5000;
    public const int MaxColumns = 64;
    public const int MaxCellLength = 4000;
    public const string DepartmentName = "DepartmentName";
    public const string DesignationName = "DesignationName";
    public const string RoleName = "RoleName";
    public const string RoleType = "RoleType";
    public const string TypeName = "TypeName";
    public const string EmployeeTypeModuleCode = "TENANT_EMPLOYEE_TYPES";
    public const string EmployeeCodeModuleCode = "TENANT_EMPLOYEE_CODE";
    public const string EmployeeModuleCode = "EMP_LIST";
    public const string EmployeeCodeLockPrefix = "employee-code-pattern:";
    public const string EmployeeLoginLockPrefix = "employee-login:";
    public const string EmployeeCode = "EmployeeCode";
    public static readonly string[] EmployeeColumns =
    [
        EmployeeCode, "FirstName", "LastName", "OfficialEmail", "DateOfBirth", "DateOfOnBoarding",
        "GenderId", "CountryId", "DepartmentId", "DesignationId", "EmployeeTypeId", "RoleId",
        "HasPermanent", IsActive, "MiddleName", "MobileNumber", "ContactName", "ContactNumber",
        "AlternateNumber", "ContactEmail", "ContactCountryId", "StateId", "DistrictId",
        "HouseNo", "Street", "LandMark", "Address"
    ];
    public const string Description = "Description";
    public const string Remark = "Remark";
    public const string IsActive = "IsActive";
}
