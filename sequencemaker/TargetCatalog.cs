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

        // Catalog-center J2000 coordinates; position angle 0 until framed in N.I.N.A.
        ["M31"] = new("00", "42", "44", "41", "16", "09", 0.0),                 // Andromeda (whole)
        ["NorthAmerica"] = new("20", "59", "17", "44", "31", "44", 0.0),        // NGC 7000 (whole)
        ["M33"] = new("01", "33", "51", "30", "39", "37", 0.0),                 // Triangulum
        ["IC1396"] = new("21", "39", "06", "57", "30", "00", 0.0),              // IC 1396 whole complex
        ["WesternVeil"] = new("20", "45", "38", "30", "42", "30", 0.0),         // NGC 6960
        ["Pelican"] = new("20", "50", "48", "44", "21", "00", 0.0),             // IC 5070
        ["EasternVeil"] = new("20", "56", "24", "31", "43", "00", 0.0),         // NGC 6992
        ["Pacman"] = new("00", "52", "59", "56", "37", "19", 0.0),              // NGC 281
        ["California"] = new("04", "03", "18", "36", "25", "18", 0.0),          // NGC 1499
        ["Veil"] = new("20", "51", "00", "30", "40", "00", 0.0),                // Veil overall
        ["FlamingStar"] = new("05", "16", "12", "34", "16", "00", 0.0),         // IC 405
        ["Jellyfish"] = new("06", "17", "13", "22", "31", "05", 0.0),           // IC 443
        ["CygnusLoop"] = new("20", "51", "00", "30", "40", "00", 0.0),          // Sh2-103
        ["Merope"] = new("03", "46", "10", "23", "45", "54", 0.0),              // NGC 1435
        ["Wizard"] = new("22", "47", "21", "58", "07", "54", 0.0),              // Sh2-142
        ["Spaghetti"] = new("05", "40", "00", "28", "00", "00", 0.0),           // Sh2-240
        ["FlyingBat"] = new("21", "11", "48", "59", "59", "00", 0.0),           // Sh2-129
    };

    public static bool TryGet(string name, out TargetInfo info) =>
        Targets.TryGetValue(name, out info!);
}

readonly record struct TargetInfo(
    string RAHours, string RAMinutes, string RASeconds,
    string DecDegrees, string DecMinutes, string DecSeconds,
    double PositionAngle);
