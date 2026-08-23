// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Maps Host-managed device domain records to safe API response contracts.
// ================================================================

using AutoMapper;
using axionpro.application.DTOS.Host;
using axionpro.domain.Entity;

namespace axionpro.application.Common.Helpers;

/// <summary>Provides response mapping for Host DeviceMaster and TenantDevice API results.</summary>
public static class HostDeviceResponseMapper
{
    /// <summary>Maps a device model and resolves its enum display name.</summary>
    public static DeviceMasterResponseDTO ToResponse(IMapper mapper, DeviceMaster entity)
    {
        var response = mapper.Map<DeviceMasterResponseDTO>(entity);
        response.DeviceTypeName = entity.DeviceType.ToString();
        return response;
    }

    /// <summary>Maps a physical device with its joined Tenant, location, model, and telemetry context.</summary>
    public static TenantDeviceResponseDTO ToResponse(IMapper mapper, TenantDevice entity)
    {
        var response = mapper.Map<TenantDeviceResponseDTO>(entity);
        response.TenantName = entity.Tenant?.CompanyName;
        response.TenantLocationName = entity.TenantLocation?.LocationName;
        response.LocationCode = entity.TenantLocation?.LocationCode;
        response.DeviceMasterName = entity.DeviceMaster?.DeviceName;
        response.DeviceMasterModelNo = entity.DeviceMaster?.ModelNo;
        response.CommunicationTypeName = entity.CommunicationType?.ToString();
        return response;
    }
}
