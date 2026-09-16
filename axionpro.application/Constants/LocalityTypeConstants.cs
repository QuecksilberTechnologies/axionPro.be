// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines stable LocalityType identifiers and names.
// ================================================================

namespace axionpro.application.Constants;

/// <summary>Canonical LocalityType values shared by persistence and API behavior.</summary>
public static class LocalityTypeConstants
{
    public const int CityId = 1;
    public const int TownId = 2;
    public const int VillageId = 3;

    public const string CityName = "City";
    public const string TownName = "Town";
    public const string VillageName = "Village";

    public static readonly int[] Ids = [CityId, TownId, VillageId];

    public static readonly IReadOnlyDictionary<int, string> Values =
        new Dictionary<int, string>
        {
            [CityId] = CityName,
            [TownId] = TownName,
            [VillageId] = VillageName
        };
}
