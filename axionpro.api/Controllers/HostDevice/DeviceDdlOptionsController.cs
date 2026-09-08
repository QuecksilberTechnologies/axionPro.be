// ================================================================
// Purpose : Exposes the firmware-confirmed dropdown values used by the Tenant
//           device-configuration UI. Angular must not hard-code device codes.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Provides a separate option endpoint for every Device Configuration UI section.</summary>
[Authorize]
[ApiController]
[Route("api/device-ddl-options")]
public sealed class DeviceDdlOptionsController : ControllerBase
{
    /// <summary>Gets time, date, NTP, and time-zone options.</summary>
    [HttpGet("time")]
    public IActionResult GetTime() => Ok(Section(DeviceDdl.Time));

    /// <summary>Gets bell output and ring-pattern options.</summary>
    [HttpGet("bell")]
    public IActionResult GetBell() => Ok(Section(DeviceDdl.Bell));

    /// <summary>Gets device display, language, and recognition options.</summary>
    [HttpGet("device-setup")]
    public IActionResult GetDeviceSetup() => Ok(Section(DeviceDdl.DeviceSetup));

    /// <summary>Gets verification, QR, and fill-light options.</summary>
    [HttpGet("advanced")]
    public IActionResult GetAdvanced() => Ok(Section(DeviceDdl.Advanced));

    /// <summary>Gets door, Wiegand, and card-display options.</summary>
    [HttpGet("lock")]
    public IActionResult GetLock() => Ok(Section(DeviceDdl.Lock));

    /// <summary>Gets serial baud-rate and function options.</summary>
    [HttpGet("serial")]
    public IActionResult GetSerial() => Ok(Section(DeviceDdl.Serial));

    /// <summary>Gets Ethernet options.</summary>
    [HttpGet("ethernet")]
    public IActionResult GetEthernet() => Ok(Section(DeviceDdl.Ethernet));

    /// <summary>Gets Wi-Fi options.</summary>
    [HttpGet("wifi")]
    public IActionResult GetWifi() => Ok(Section(DeviceDdl.Wifi));

    /// <summary>Gets app-notification options.</summary>
    [HttpGet("app-notification")]
    public IActionResult GetAppNotification() => Ok(Section(DeviceDdl.AppNotification));

    private static ApiResponse<IReadOnlyList<DeviceDdlField>> Section(string section) =>
        ApiResponse<IReadOnlyList<DeviceDdlField>>.Success(DeviceDdl.GetSection(section));
}
