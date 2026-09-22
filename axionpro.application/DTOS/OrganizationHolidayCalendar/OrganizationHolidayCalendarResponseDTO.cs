namespace axionpro.application.DTOs.OrganizationHolidayCalendar;

public sealed class OrganizationHolidayCalendarDTO
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long TenantLocationId { get; set; }

    public string HolidayName { get; set; } = string.Empty;

    public DateOnly HolidayDate { get; set; }

    public bool IsOptional { get; set; }

    public string? Description { get; set; }
}
