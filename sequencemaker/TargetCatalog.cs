static class TargetCatalog
{
    public static readonly Dictionary<string, TargetInfo> Targets = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pleiades"] = new("03", "46", "18", "24", "08", "04", 208.30),
        ["gammacygni"] = new("20", "24", "51", "39", "59", "48", 0.0),
        ["M31-Panel1"] = new("00", "37", "25", "40", "34", "48", 236.96),
        ["M31-Panel2"] = new("00", "42", "36", "41", "12", "28", 237.80),
        ["M31-Panel3"] = new("00", "47", "53", "41", "49", "15", 238.68),
        ["NAN-Panel1"] = new("20", "53", "11", "44", "06", "19", 265.34),
        ["NAN-Panel2"] = new("20", "59", "39", "44", "11", "19", 266.46),
        ["HeartNebula"] = new("02", "33", "17", "61", "11", "57", 114.9),
        ["ElephantTrunk"] = new("21", "34", "53", "57", "30", "30", 269.2),
        ["TadpoleNebula"] = new("05", "22", "31", "33", "25", "45", 213.7),
    };

    public static bool TryGet(string name, out TargetInfo info) =>
        Targets.TryGetValue(name, out info!);
}

readonly record struct TargetInfo(
    string RAHours, string RAMinutes, string RASeconds,
    string DecDegrees, string DecMinutes, string DecSeconds,
    double PositionAngle);
