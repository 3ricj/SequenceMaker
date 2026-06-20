
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

        string targetObject = "gammacygni";

        //string[] filterSequence = ["Red", "Green", "Blue", "Luminance"];
        //string[] filterSequence = ["Red", "Green", "Blue"];
        //string[] filterSequence = ["Luminance"];
        string[] filterSequence = ["H-Alpha", "SII", "OIII"];
        //string[] filterSequence = ["H-Alpha"];
        //string[] filterSequence = ["SII", "OIII"];

        // Each ramp = ~20 min (RGB) or ~1 hr (Sequence1/Sequence5). See SequenceRamps.cs.
        List<double[]> sequenceList = [SequenceRamps.Sequence1];
        //List<double[]> sequenceList = [SequenceRamps.Sequence1];                                                 // 1 hr (narrowband)
        //List<double[]> sequenceList = [SequenceRamps.Sequence5];                                                 // 1 hr (Luminance)
        //List<double[]> sequenceList = [SequenceRamps.Sequence1, SequenceRamps.Sequence1, SequenceRamps.Sequence1]; // 3 hr

        string fileNameBase = "2026-06-19-shootlist-Sequence1-gammacygni-HASIIOIII";

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
            TargetObject = targetObject,
            FilterSequence = filterSequence,
            SequenceList = sequenceList,
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

        int hours = (int)(totalSeconds / 3600);
        int minutes = (int)((totalSeconds % 3600) / 60);
        int seconds = (int)(totalSeconds % 60);
        Console.WriteLine($"Frame total: {frameCount}, taking: {hours} hours, {minutes} minutes, and {seconds} seconds.");

        if (lastOutput != null)
            FilePathHelper.OpenExplorerSelect(lastOutput);
    }
}
