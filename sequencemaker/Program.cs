
class Program
{
    static void Main()
    {
        // ================= Tonight's shoot - edit these. =================
        // Exposure ramp patterns (Sequence1..Sequence5) are defined in SequenceRamps.cs.
        // Known targets and their coordinates are defined in TargetCatalog.cs.
        //
        // Exposure ramps (each value = one frame's exposure in seconds):
        //   Sequence1 - one hour, ~1s..60s ramp. For narrowband (or any 1hr block).
        //   Sequence2 - second hour continuation, ~50s..60s. For any filter.
        //   Sequence3 - RGB ~20 min, ~1s..60s ramp with dither markers.
        //   Sequence4 - RGB ~20 min second hour, ~50s..60s.
        //   Sequence5 - one hour, ~1s..60s ramp with dither markers. For Luminance.
        // (Dither markers are the tiny 0.001/0.002/... values that trigger a dither.)

        // Targets are shot in order, each with its own slew / center / autofocus.
        // Blocks within a target are shot in the order listed: (filter, ramp).
        // Only the LAST target loops until Nautical Dawn; earlier targets run once.
        // Filter names: Red, Green, Blue, Luminance, H-Alpha, SII, OIII.
        List<TargetPlan> targets =
        [
            new("EasternVeil",                                  // 4h
            [
                new("H-Alpha",   SequenceRamps.Sequence1),      // 1 hr
                new("OIII",      SequenceRamps.Sequence1),      // 1 hr
                new("SII",       SequenceRamps.Sequence1),      // 1 hr
                new("Red",       SequenceRamps.Sequence3),      // 20 min
                new("Green",     SequenceRamps.Sequence3),      // 20 min
                new("Blue",      SequenceRamps.Sequence3),      // 20 min
            ]),
            new("M31-Panel2",                                   // 1h
            [
                new("Luminance", SequenceRamps.Sequence3),      // 20 min
                new("Red",       SequenceRamps.Sequence3),      // 20 min
                new("Blue",      SequenceRamps.Sequence3),      // 20 min
            ]),
            new("FlamingStar",                                  // 5h
            [
                new("Red",       SequenceRamps.Sequence3),      // 20 min
                new("Red",       SequenceRamps.Sequence4),      // 20 min
                new("Green",     SequenceRamps.Sequence3),      // 20 min
                new("Green",     SequenceRamps.Sequence4),      // 20 min
                new("Blue",      SequenceRamps.Sequence3),      // 20 min
                new("Blue",      SequenceRamps.Sequence4),      // 20 min
                new("H-Alpha",   SequenceRamps.Sequence1),      // 1 hr
                new("OIII",      SequenceRamps.Sequence1),      // 1 hr
                new("SII",       SequenceRamps.Sequence1),      // 1 hr
            ]),
        ];

        // Old style (every ramp on every filter, one target):
        //List<TargetPlan> targets = [TargetPlan.EveryFilter("gammacygni", ["H-Alpha", "SII", "OIII"], [SequenceRamps.Sequence2])];

        string fileNameBase = "2026-10-13-shootlist-EasternVeil-M31Panel2-FlamingStar";

        // Which files to write:
        bool writeXml = true;
        bool writeJson = true;

        // Sequence Start / End templates. null = use the bundled defaults in sequencemaker/templates.
        // To use one of your own N.I.N.A. templates instead, set its template name here.
        string? startTemplateName = null;
        string? endTemplateName = null;
        // =================================================================

        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var config = new SequenceConfig
        {
            Targets = targets,
            FileNameBase = fileNameBase,
            StartTemplateName = startTemplateName,
            EndTemplateName = endTemplateName,
            NinaOutputFolder = Path.Combine(documentsPath, "N.I.N.A")
        };

        Directory.CreateDirectory(config.NinaOutputFolder);

        string xmlPath = Path.Combine(config.NinaOutputFolder, config.FileNameBase + ".xml");
        string jsonPath = Path.Combine(config.NinaOutputFolder, config.FileNameBase + ".json");

        int frameCount = config.ExpectedFrameCount;
        double totalSeconds = 0;
        string? lastOutput = null;

        if (writeXml)
        {
            var xmlResult = XmlSequenceWriter.Write(config, xmlPath);
            frameCount = xmlResult.frameCount;
            totalSeconds = xmlResult.totalExposureSeconds;
            lastOutput = xmlResult.filePath;
        }

        if (writeJson)
        {
            lastOutput = JsonSequenceWriter.Write(config, jsonPath);
        }

        foreach (var plan in config.Targets)
        {
            var span = TimeSpan.FromSeconds(plan.ExposureSeconds);
            Console.WriteLine($"  {plan.TargetName}: {plan.FrameCount} frames, {(int)span.TotalHours}h{span.Minutes:00}m exposure");
        }

        int hours = (int)(totalSeconds / 3600);
        int minutes = (int)((totalSeconds % 3600) / 60);
        int seconds = (int)(totalSeconds % 60);
        Console.WriteLine($"Frame total: {frameCount}, taking: {hours} hours, {minutes} minutes, and {seconds} seconds.");

        if (lastOutput != null)
            FilePathHelper.OpenExplorerSelect(lastOutput);
    }
}
