using ChoETL;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Data.Common;



class Program
{
 

    static double[] sequence1_old = [1.0, 1.018, 1.037, 1.056, 1.076, 1.096, 1.116, 1.1360000000000001, 1.157, 1.179, 1.201, 1.223, 1.245, 1.268, 1.292, 1.315, 1.34, 1.364, 1.3900000000000001, 1.415, 1.441, 1.468, 1.495, 1.5230000000000001, 1.551, 1.579, 1.608, 1.6380000000000001, 1.668, 1.699, 1.73, 1.762, 1.795, 1.828, 1.862, 1.8960000000000001, 1.931, 1.967, 2.003, 2.04, 2.077, 2.116, 2.1550000000000002, 2.195, 2.235, 2.2760000000000002, 2.318, 2.361, 2.4050000000000002, 2.449, 2.494, 2.54, 2.587, 2.6350000000000002, 2.6830000000000003, 2.733, 2.783, 2.834, 2.887, 2.94, 2.994, 3.0500000000000003, 3.106, 3.1630000000000003, 3.221, 3.281, 3.341, 3.403, 3.466, 3.5300000000000002, 3.595, 3.661, 3.729, 3.797, 3.867, 3.939, 4.011, 4.085, 4.1610000000000005, 4.238, 4.316, 4.3950000000000005, 4.476, 4.559, 4.643, 4.729, 4.816, 4.905, 4.995, 5.087, 5.181, 5.277, 5.3740000000000006, 5.473, 5.574, 5.6770000000000005, 5.782, 5.888, 5.997, 6.1080000000000005, 6.22, 6.335, 6.452, 6.571, 6.692, 6.816, 6.941, 7.069, 7.2, 7.333, 7.468, 7.606, 7.746, 7.889, 8.034, 8.183, 8.334, 8.487, 8.644, 8.803, 8.966, 9.131, 9.299, 9.471, 9.646, 9.824, 10.005, 10.189, 10.377, 10.569, 10.764, 10.962, 11.164, 11.370000000000001, 11.58, 11.794, 12.011000000000001, 12.233, 12.459, 12.688, 12.922, 13.161, 13.404, 13.651, 13.903, 14.159, 14.42, 14.686, 14.957, 15.233, 15.514000000000001, 15.8, 16.092, 16.389, 16.691, 16.999, 17.312, 17.632, 17.957, 18.288, 18.626, 18.969, 19.319, 19.675, 20.038, 20.408, 20.784, 21.168, 21.558, 21.956, 22.361, 22.773, 23.193, 23.621000000000002, 24.057000000000002, 24.501, 24.953, 25.413, 25.882, 26.359, 26.846, 27.341, 27.845, 28.359, 28.882, 29.415, 29.957, 30.51, 31.073, 31.646, 32.229, 32.824, 33.429, 34.046, 34.674, 35.314, 35.965, 36.629, 37.304, 37.992, 38.693, 39.407000000000004, 40.134, 40.874, 41.628, 42.396, 43.178000000000004, 43.975, 44.786, 45.612, 46.453, 47.31, 48.183, 49.072, 49.977000000000004, 50.899, 51.838, 52.794000000000004, 53.768, 54.76, 55.77, 56.798, 57.846000000000004, 58.913000000000004, 60.0];

    static double[] sequence2_old = [50.0, 50.146, 50.293, 50.44, 50.588, 50.735, 50.884, 51.033, 51.182, 51.332, 51.482, 51.632, 51.783, 51.935, 52.087, 52.239000000000004, 52.392, 52.545, 52.699, 52.853, 53.007, 53.162, 53.318, 53.474000000000004, 53.63, 53.787, 53.944, 54.102000000000004, 54.26, 54.419000000000004, 54.578, 54.738, 54.898, 55.059000000000005, 55.22, 55.381, 55.543, 55.705, 55.868, 56.032000000000004, 56.196, 56.36, 56.525, 56.69, 56.856, 57.022, 57.189, 57.356, 57.524, 57.692, 57.861000000000004, 58.03, 58.2, 58.370000000000005, 58.541000000000004, 58.712, 58.884, 59.056000000000004, 59.229, 59.402, 59.576, 59.75, 59.925000000000004, 60.1];

    static double[] sequence3_old = [1.0, 1.058, 1.119, 1.183, 1.252, 1.324, 1.4000000000000001, 1.481, 1.566, 1.657, 1.752, 1.853, 1.96, 2.073, 2.193, 2.319, 2.453, 2.595, 2.744, 2.903, 3.0700000000000003, 3.247, 3.435, 3.633, 3.842, 4.064, 4.298, 4.546, 4.809, 5.086, 5.38, 5.69, 6.018, 6.365, 6.7330000000000005, 7.121, 7.532, 7.966, 8.426, 8.912, 9.426, 9.97, 10.545, 11.153, 11.797, 12.477, 13.197000000000001, 13.958, 14.764000000000001, 15.615, 16.516000000000002, 17.469, 18.477, 19.543, 20.67, 21.863, 23.124, 24.458000000000002, 25.869, 27.361, 28.94, 30.609, 32.375, 34.243, 36.218, 38.308, 40.518, 42.855000000000004, 45.327, 47.942, 50.708, 53.633, 56.727000000000004, 60.0];

    static double[] sequence4_old = [50.0, 50.462, 50.928000000000004, 51.399, 51.874, 52.354, 52.837, 53.326, 53.819, 54.316, 54.818, 55.324, 55.836, 56.352000000000004, 56.873000000000005, 57.398, 57.929, 58.464, 59.004, 59.550000000000004, 60.1];

    //"Sequence 1 - one hour - for narrowband only"
    static double[] sequence1 = [1.0, 1.018, 1.037, 1.056, 1.076, 1.096, 1.116, 1.1360000000000001, 1.157, 1.179, 1.201, 1.223, 1.245, 1.268, 1.292, 1.315, 1.34, 1.364, 1.3900000000000001, 1.415, 1.441, 1.468, 1.495, 1.5230000000000001, 1.551, 1.579, 1.608, 1.6380000000000001, 1.668, 1.699, 1.73, 1.762, 1.795, 1.828, 1.862, 1.8960000000000001, 1.931, 1.967, 2.003, 2.04, 2.077, 2.116, 2.1550000000000002, 2.195, 2.235, 2.2760000000000002, 2.318, 2.361, 2.4050000000000002, 2.449, 2.494, 2.54, 2.587, 2.6350000000000002, 2.6830000000000003, 2.733, 2.783, 2.834, 2.887, 2.94, 2.994, 3.0500000000000003, 3.106, 3.1630000000000003, 3.221, 3.281, 3.341, 3.403, 3.466, 3.5300000000000002, 3.595, 3.661, 3.729, 3.797, 3.867, 3.939, 4.011, 4.085, 4.1610000000000005, 4.238, 4.316, 4.3950000000000005, 4.476, 4.559, 4.643, 4.729, 4.816, 4.905, 4.995, 5.087, 5.181, 5.277, 5.3740000000000006, 5.473, 5.574, 5.6770000000000005, 5.782, 5.888, 5.997, 6.1080000000000005, 6.22, 6.335, 6.452, 6.571, 6.692, 6.816, 6.941, 7.069, 7.2, 7.333, 7.468, 7.606, 7.746, 7.889, 8.034, 8.183, 8.334, 8.487, 8.644, 8.803, 8.966, 9.131, 9.299, 9.471, 9.646, 9.824, 10.005, 10.189, 10.377, 10.569, 10.764, 10.962, 11.164, 11.370000000000001, 11.58, 11.794, 12.011000000000001, 12.233, 12.459, 12.688, 12.922, 13.161, 13.404, 13.651, 13.903, 14.159, 14.42, 14.686, 14.957, 15.233, 15.514000000000001, 15.8, 16.092, 16.389, 16.691, 16.999, 17.312, 17.632, 17.957, 18.288, 18.626, 18.969, 19.319, 19.675, 20.038, 20.408, 20.784, 21.168, 21.558, 21.956, 22.361, 22.773, 23.193, 23.621000000000002, 24.057000000000002, 24.501, 24.953, 25.413, 25.882, 26.359, 26.846, 27.341, 27.845, 28.359, 28.882, 29.415, 29.957, 30.51, 31.073, 31.646, 32.229, 32.824, 33.429, 34.046, 34.674, 35.314, 35.965, 36.629, 37.304, 37.992, 38.693, 39.407000000000004, 40.134, 40.874, 41.628, 42.396, 43.178000000000004, 43.975, 44.786, 45.612, 46.453, 47.31, 48.183, 49.072, 49.977000000000004, 50.899, 51.838, 52.794000000000004, 53.768, 54.76, 55.77, 56.798, 57.846000000000004, 58.913000000000004, 60.0];
    //"Sequence 2 - for 2nd hour - for any filter"
    static double[] sequence2 = [50.0, 50.146, 50.293, 50.44, 50.588, 50.735, 50.884, 51.033, 51.182, 51.332, 51.482, 51.632, 51.783, 51.935, 52.087, 52.239000000000004, 52.392, 52.545, 52.699, 52.853, 53.007, 53.162, 53.318, 53.474000000000004, 53.63, 53.787, 53.944, 54.102000000000004, 54.26, 54.419000000000004, 54.578, 54.738, 54.898, 55.059000000000005, 55.22, 55.381, 55.543, 55.705, 55.868, 56.032000000000004, 56.196, 56.36, 56.525, 56.69, 56.856, 57.022, 57.189, 57.356, 57.524, 57.692, 57.861000000000004, 58.03, 58.2, 58.370000000000005, 58.541000000000004, 58.712, 58.884, 59.056000000000004, 59.229, 59.402, 59.576, 59.75, 59.925000000000004, 60.1];
    //"Sequence 3 (new) - RGB 20 min",
    static double[] sequence3 = [1.0, 1.058, 1.119, 1.183, 1.252, 1.324, 1.4000000000000001, 1.481, 1.566, 1.657, 1.752, 1.853, 1.96, 2.073, 2.193, 2.319, 2.453, 2.595, 2.744, 0.001, 2.903, 3.0700000000000003, 3.247, 0.002, 3.435, 3.633, 3.842, 0.004, 4.064, 4.298, 4.546, 0.008, 4.809, 5.086, 5.38, 0.016, 5.69, 6.018, 6.365, 0.032, 6.7330000000000005, 7.121, 7.532, 0.064, 7.966, 8.426, 8.912, 0.128, 9.426, 9.97, 10.545, 0.256, 11.153, 11.797, 12.477, 0.512, 13.197000000000001, 13.958, 14.764000000000001, 15.615, 16.516000000000002, 17.469, 18.477, 19.543, 20.67, 21.863, 23.124, 24.458000000000002, 25.869, 27.361, 28.94, 30.609, 32.375, 34.243, 36.218, 38.308, 40.518, 42.855000000000004, 45.327, 47.942, 50.708, 53.633, 56.727000000000004, 60.0];
    //"Sequence 4 - RGB 20 min for 2nd hour",
    static double[] sequence4 = [50.0, 50.462, 50.928000000000004, 51.399, 51.874, 52.354, 52.837, 53.326, 53.819, 54.316, 54.818, 55.324, 55.836, 56.352000000000004, 56.873000000000005, 57.398, 57.929, 58.464, 59.004, 59.550000000000004, 60.1];
    //"Sequence 5 (new) - one hour - for L band",
    static double[] sequence5 = [1.0, 1.018, 1.037, 1.056, 1.076, 1.096, 1.116, 1.1360000000000001, 1.157, 1.179, 1.201, 1.223, 1.245, 1.268, 1.292, 1.315, 1.34, 1.364, 1.3900000000000001, 1.415, 1.441, 1.468, 1.495, 1.5230000000000001, 1.551, 1.579, 1.608, 1.6380000000000001, 1.668, 1.699, 1.73, 1.762, 1.795, 1.828, 1.862, 1.8960000000000001, 1.931, 1.967, 2.003, 2.04, 2.077, 2.116, 2.1550000000000002, 2.195, 2.235, 2.2760000000000002, 2.318, 2.361, 2.4050000000000002, 2.449, 2.494, 2.54, 2.587, 2.6350000000000002, 2.6830000000000003, 2.733, 2.783, 2.834, 2.887, 2.94, 2.994, 3.0500000000000003, 3.106, 3.1630000000000003, 3.221, 3.281, 3.341, 3.403, 3.466, 3.5300000000000002, 3.595, 3.661, 3.729, 3.797, 3.867, 3.939, 4.011, 4.085, 4.1610000000000005, 4.238, 4.316, 4.3950000000000005, 4.476, 4.559, 4.643, 4.729, 4.816, 4.905, 4.995, 5.087, 5.181, 5.277, 5.3740000000000006, 5.473, 5.574, 5.6770000000000005, 5.782, 5.888, 5.997, 0.001, 6.1080000000000005, 6.22, 6.335, 6.452, 6.571, 0.002, 6.692, 6.816, 6.941, 7.069, 7.2, 0.004, 7.333, 7.468, 7.606, 7.746, 7.889, 0.008, 8.034, 8.183, 8.334, 8.487, 8.644, 0.016, 8.803, 8.966, 9.131, 9.299, 9.471, 0.032, 9.646, 9.824, 10.005, 10.189, 10.377, 0.064, 10.569, 10.764, 10.962, 11.164, 11.370000000000001, 0.128, 11.58, 11.794, 12.011000000000001, 12.233, 12.459, 0.256, 12.688, 12.922, 13.161, 13.404, 13.651, 0.512, 13.903, 14.159, 14.42, 14.686, 14.957, 15.233, 15.514000000000001, 15.8, 16.092, 16.389, 16.691, 16.999, 17.312, 17.632, 17.957, 18.288, 18.626, 18.969, 19.319, 19.675, 20.038, 20.408, 20.784, 21.168, 21.558, 21.956, 22.361, 22.773, 23.193, 23.621000000000002, 24.057000000000002, 24.501, 24.953, 25.413, 25.882, 26.359, 26.846, 27.341, 27.845, 28.359, 28.882, 29.415, 29.957, 30.51, 31.073, 31.646, 32.229, 32.824, 33.429, 34.046, 34.674, 35.314, 35.965, 36.629, 37.304, 37.992, 38.693, 39.407000000000004, 40.134, 40.874, 41.628, 42.396, 43.178000000000004, 43.975, 44.786, 45.612, 46.453, 47.31, 48.183, 49.072, 49.977000000000004, 50.899, 51.838, 52.794000000000004, 53.768, 54.76, 55.77, 56.798, 57.846000000000004, 58.913000000000004, 60.0];

    // Define the dictionary for target details
    static Dictionary<string, (string RAHours, string RAMinutes, string RASeconds, string DecDegrees, string DecMinutes, string DecSeconds, string PositionAngle)> targetDetails =
        new Dictionary<string, (string, string, string, string, string, string, string)>
    {
        { "pleiades", ("03", "46", "18", "24", "08", "04", "208.30") },
        { "gammacygni", ("20", "24", "51", "39", "59", "48", "360.00") },
        { "M31-Panel1", ("00", "37", "25", "40", "34", "48", "236.96") },
        { "M31-Panel2", ("00", "42", "36", "41", "12", "28", "237.80") },
        { "M31-Panel3", ("00", "47", "53", "41", "49", "15", "238.68") },
        { "NAN-Panel1", ("20", "53", "11", "44", "06", "19", "265.34") },
        { "NAN-Panel2", ("20", "59", "39", "44", "11", "19", "266.46") },
        { "HeartNebula", ("02", "33", "17", "61", "11", "57", "114.9") },
        { "ElephantTrunk", ("21", "34", "53", "57", "30", "30", "269.2") }, 
        { "TapoleNebula", ("05", "22", "31", "33", "25", "45", "213.7") },

        };

    static void Main(string[] args)
    {
        string[] FilterSequence;
        string targetObject;

        //string[] FilterSequence = ["Red", "Green", "Blue", "Luminance"];

        //FilterSequence = ["Red", "Green", "Blue"];
        //FilterSequence = ["Luminance"];
        FilterSequence = [
            "H-Alpha", 
            "SII", 
            "OIII"
            ];
        //FilterSequence = ["H-Alpha"];
        //FilterSequence = ["SII", "OIII"];

        //FilterSequence = ["Luminance"];

        List<double[]> sequenceList = [sequence2]; //, sequence2];

        targetObject = "TapoleNebula";


        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string fileName = $"2024-10-11-shootlist-Sequence2-{targetObject}-HASIIOIII.xml";
        string filePath = Path.Combine(documentsPath, "N.I.N.A", fileName);


        filePath = EnsureUniqueFilePath(filePath);
        Console.WriteLine("Saved to: " + filePath);


        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        //settings.ConformanceLevel = ConformanceLevel.Fragment;

        XmlWriter writer = XmlWriter.Create(filePath, settings);
        writer.WriteStartDocument();
        writer.WriteStartElement("CaptureSequenceList");

        writer = WriteTargetDetails(writer, targetObject);


        const double overheadTime = 1.5; // Overhead time in seconds
        double totalExposureTimeInSeconds = 0; // Accumulated total time in seconds
        int framecount = 0;


        foreach (string FilterSelect in FilterSequence)
        {
            foreach (var sequence in sequenceList) // Iterate through each sequence
            {
                foreach (double exposure_time in sequence)
                {
                    double currentExposureTime = exposure_time + overheadTime;
                    totalExposureTimeInSeconds += currentExposureTime;
                    framecount++;

                    Console.WriteLine("Amount is {0}", exposure_time);
                    writer.WriteStartElement("CaptureSequence");
                    writer.WriteElementString("Enabled", "true");
                    writer.WriteElementString("ExposureTime", exposure_time.ToString("0.000"));
                    writer.WriteElementString("ImageType", "FLAT");

                    writer.WriteStartElement("FilterType");
                    writer.WriteElementString("Name", FilterSelect);
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

        /*
        foreach (double exposure_time in sequence2)
        {
            Console.WriteLine("Amount is {0}", exposure_time);
            writer.WriteStartElement("CaptureSequence");
            writer.WriteElementString("Enabled", "true");
            writer.WriteElementString("ExposureTime", exposure_time.ToString("0.000"));
            writer.WriteElementString("ImageType", "FLAT");

            writer.WriteStartElement("FilterType");
            writer.WriteElementString("Name", "H-Alpha");
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

        */

        //writer.WriteStartElement("Coordinates");
        //writer.WriteElementString("RA", "0.7123138886666667");
        //writer.WriteElementString("Dec", "41.26875");
        //writer.WriteElementString("Epoch", "J2000");
        //writer.WriteEndElement();


        writer.WriteEndElement();


        writer.WriteEndDocument();
        writer.Flush();
        writer.Close();


//        double totalTimeInHours = totalExposureTimeInSeconds / 3600;
//        int hours = (int)Math.Floor(totalTimeInHours);
//        int minutes = (int)Math.Round((totalTimeInHours - hours) * 60);
//        int seconds = (int)Math.Round(totalTimeInHours - hours - minute)

        int hours = (int)(totalExposureTimeInSeconds / 3600);
        int minutes = (int)((totalExposureTimeInSeconds % 3600) / 60);
        int seconds = (int)(totalExposureTimeInSeconds % 60);

        Console.WriteLine($"Frame total: {framecount}, taking: {hours} hours, {minutes} minutes, and {seconds} seconds.");



        static XmlWriter WriteTargetDetails(XmlWriter writer, string targetName)
        {
            // Lookup the target details in the dictionary
            if (targetDetails.TryGetValue(targetName, out var details))
            {
//                writer.WriteStartElement("Target");
                writer.WriteAttributeString("TargetName", targetName);
                writer.WriteAttributeString("RAHours", details.RAHours);
                writer.WriteAttributeString("RAMinutes", details.RAMinutes);
                writer.WriteAttributeString("RASeconds", details.RASeconds);
                writer.WriteAttributeString("DecDegrees", details.DecDegrees);
                writer.WriteAttributeString("DecMinutes", details.DecMinutes);
                writer.WriteAttributeString("DecSeconds", details.DecSeconds);
                writer.WriteAttributeString("PositionAngle", details.PositionAngle);
                writer.WriteAttributeString("SlewToTarget", "true");
                writer.WriteAttributeString("AutoFocusOnStart", "true");
                writer.WriteAttributeString("CenterTarget", "true");
                writer.WriteAttributeString("RotateTarget", "true");
                writer.WriteAttributeString("AutoFocusOnFilterChange", "false");
                writer.WriteAttributeString("AutoFocusAfterTemperatureChange", "false");
                writer.WriteAttributeString("AutoFocusAfterTemperatureChangeAmount", "20");
//                writer.WriteEndElement();
            }
            else
            {
                Console.WriteLine($"Target '{targetName}' not found in the dictionary.");
            }

            return writer;
        }

        static string EnsureUniqueFilePath(string filePath)
        {
            // If the file already exists, append a timestamp to avoid overwriting it
            if (File.Exists(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);

                // Append a timestamp to the file name
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string newFileName = $"{filenameWithoutExtension}-{timestamp}{extension}";
                return Path.Combine(directory, newFileName);
            }

            return filePath;
        }
    }
}
