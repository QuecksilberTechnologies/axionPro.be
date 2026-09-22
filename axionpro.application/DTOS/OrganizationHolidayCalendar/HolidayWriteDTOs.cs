using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOs.OrganizationHolidayCalendar;

public sealed class HolidayByIdRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }
}

public sealed class SaveHolidayRequestDTO : PermissionRequestDTO
{
    public long TenantLocationId { get; set; }

    public string HolidayName { get; set; } = string.Empty;

    public DateOnly HolidayDate { get; set; }

    public bool IsOptional { get; set; }

    public string? Description { get; set; }
}

public sealed class UpdateHolidayRequestDTO : PermissionRequestDTO
{
    public long Id { get; set; }

    public long TenantLocationId { get; set; }

    public string HolidayName { get; set; } = string.Empty;

    public DateOnly HolidayDate { get; set; }

    public bool IsOptional { get; set; }

    public string? Description { get; set; }
}

public sealed class ImportHolidayRequestDTO : BulkImportPreviewRequestDTO
{
}

public sealed class HolidayImportResultDTO
{
    public int TotalRows { get; set; }

    public int CreatedCount { get; set; }

    public int SkippedExistingCount { get; set; }
}
