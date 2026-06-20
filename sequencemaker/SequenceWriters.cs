using System.Diagnostics;
using System.Xml;

static class XmlSequenceWriter
{
    public static (string filePath, int frameCount, double totalExposureSeconds) Write(
        SequenceConfig config,
        string filePath)
    {
        filePath = FilePathHelper.EnsureUnique(filePath);
        Console.WriteLine("Saved XML to: " + filePath);

        var settings = new XmlWriterSettings { Indent = true };
        using var writer = XmlWriter.Create(filePath, settings);
        writer.WriteStartDocument();
        writer.WriteStartElement("CaptureSequenceList");
        WriteTargetAttributes(writer, config.TargetObject);

        const double overheadTime = 1.5;
        double totalExposureTimeInSeconds = 0;
        int framecount = 0;

        foreach (string filterSelect in config.FilterSequence)
        {
            foreach (var sequence in config.SequenceList)
            {
                foreach (double exposureTime in sequence)
                {
                    totalExposureTimeInSeconds += exposureTime + overheadTime;
                    framecount++;
                    Console.WriteLine("Amount is {0}", exposureTime);

                    writer.WriteStartElement("CaptureSequence");
                    writer.WriteElementString("Enabled", "true");
                    writer.WriteElementString("ExposureTime", exposureTime.ToString("0.000"));
                    writer.WriteElementString("ImageType", "FLAT");
                    writer.WriteStartElement("FilterType");
                    writer.WriteElementString("Name", filterSelect);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Binning");
                    writer.WriteElementString("X", "1");
                    writer.WriteElementString("Y", "1");
                    writer.WriteEndElement();
                    writer.WriteElementString("Gain", "-1");
                    writer.WriteElementString("Offset", "-1");
                    writer.WriteElementString("TotalExposureCount", "1");
                    writer.WriteElementString("ProgressExposureCount", "0");
                    writer.WriteElementString("Dither", "false");
                    writer.WriteElementString("DitherAmount", "1");
                    writer.WriteEndElement();
                }
            }
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();

        return (filePath, framecount, totalExposureTimeInSeconds);
    }

    static void WriteTargetAttributes(XmlWriter writer, string targetName)
    {
        if (!TargetCatalog.TryGet(targetName, out var details))
        {
            Console.WriteLine($"Target '{targetName}' not found in the dictionary.");
            return;
        }

        writer.WriteAttributeString("TargetName", targetName);
        writer.WriteAttributeString("RAHours", details.RAHours);
        writer.WriteAttributeString("RAMinutes", details.RAMinutes);
        writer.WriteAttributeString("RASeconds", details.RASeconds);
        writer.WriteAttributeString("DecDegrees", details.DecDegrees);
        writer.WriteAttributeString("DecMinutes", details.DecMinutes);
        writer.WriteAttributeString("DecSeconds", details.DecSeconds);
        writer.WriteAttributeString("PositionAngle", details.PositionAngle.ToString("0.00"));
        writer.WriteAttributeString("SlewToTarget", "true");
        writer.WriteAttributeString("AutoFocusOnStart", "true");
        writer.WriteAttributeString("CenterTarget", "true");
        writer.WriteAttributeString("RotateTarget", "true");
        writer.WriteAttributeString("AutoFocusOnFilterChange", "false");
        writer.WriteAttributeString("AutoFocusAfterTemperatureChange", "false");
        writer.WriteAttributeString("AutoFocusAfterTemperatureChangeAmount", "20");
    }
}

static class JsonSequenceWriter
{
    public static string Write(SequenceConfig config, string filePath)
    {
        filePath = FilePathHelper.EnsureUnique(filePath);
        var json = new NinaAdvancedSequenceBuilder(config).BuildAndSerialize();
        File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        Console.WriteLine("Saved JSON to: " + filePath);
        return filePath;
    }
}

static class FilePathHelper
{
    public static string EnsureUnique(string filePath)
    {
        if (!File.Exists(filePath))
            return filePath;

        string? directory = Path.GetDirectoryName(filePath);
        string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);
        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string newFileName = $"{filenameWithoutExtension}-{timestamp}{extension}";
        return Path.Combine(directory ?? ".", newFileName);
    }

    public static void OpenExplorerSelect(string filePath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{filePath}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not open Explorer: " + ex.Message);
        }
    }
}
