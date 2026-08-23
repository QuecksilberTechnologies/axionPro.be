// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines supported Host-managed device-model types used by DeviceMaster.
// ================================================================

namespace axionpro.domain.Entity;

/// <summary>Identifies the primary functional type of a device model in the Host-managed catalog.</summary>
public enum DeviceType : short
{
    /// <summary>Face-recognition device.</summary>
    Face = 1,
    /// <summary>Fingerprint-recognition device.</summary>
    Fingerprint = 2,
    /// <summary>Card-based device.</summary>
    Card = 3,
    /// <summary>Device supporting face and fingerprint recognition.</summary>
    FaceFingerprint = 4,
    /// <summary>Device supporting face recognition and card verification.</summary>
    FaceCard = 5,
    /// <summary>Multi-biometric device supporting multiple verification methods.</summary>
    MultiBiometric = 6,
    /// <summary>Access-control-oriented device.</summary>
    AccessControl = 7,
    /// <summary>Device type not represented by the standard categories.</summary>
    Other = 8
}

/// <summary>Identifies the communication channel used by a physical Tenant device.</summary>
public enum DeviceCommunicationType : short
{
    /// <summary>HTTP communication.</summary>
    Http = 1,
    /// <summary>HTTPS communication.</summary>
    Https = 2,
    /// <summary>WebSocket communication.</summary>
    WebSocket = 3,
    /// <summary>TCP communication.</summary>
    Tcp = 4,
    /// <summary>Cloud API communication.</summary>
    CloudApi = 5,
    /// <summary>Push SDK communication.</summary>
    PushSdk = 6
}
