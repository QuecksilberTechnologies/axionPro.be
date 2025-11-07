using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;

[Route("api/[controller]")]
[ApiController]
public class ClientInfoController : ControllerBase
{ 
    private string GetPublicIpFromRequest()
    {
        // Agar client proxy ke through connected hai toh "X-Forwarded-For" header check karein
        string ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (string.IsNullOrEmpty(ip))
        {
            ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }

        return ip;
    }
    [HttpGet("detect-device")]
    public IActionResult GetDeviceInfo()
    {
        var userAgent = Request.Headers["User-Agent"].ToString();
        var deviceType = GetDeviceType(userAgent);
        var localIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        var publicIp = GetPublicIpFromRequest();

        //var useragent = request.headers["user-agent"].tostring();
        //var devicetype = getdevicetype(useragent);
        //var localip ="192.168.0.08";
        //var publicip = "33.454.32.344";
        var deviceInfo = new
        {
            localIp ,
            publicIp ,
            deviceType,
            userAgent
        };

        return Ok(deviceInfo);
    }

    private int GetDeviceType(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return -1;

        userAgent = userAgent.ToLower();

        if (userAgent.Contains("mobile")) return 2;
        if (userAgent.Contains("tablet")) return 3;
        if (userAgent.Contains("ipad")) return 3;
        if (userAgent.Contains("android") && !userAgent.Contains("mobile")) return 3;

        return 1;
    }
}
