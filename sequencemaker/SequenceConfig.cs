class SequenceConfig
{
    public string TargetObject { get; set; } = "gammacygni";
    public string[] FilterSequence { get; set; } = ["Red", "Green", "Blue", "Luminance"];
    public List<double[]> SequenceList { get; set; } = [];
    public string FileNameBase { get; set; } = "";
    public string? StartTemplateName { get; set; }
    public string? EndTemplateName { get; set; }
    /// <summary>Null = no failure alerts. Future: "ground-station-pushover", etc.</summary>
    public string? AlertTemplate { get; set; }
    public string NinaOutputFolder { get; set; } = "";
    public int ExpectedFrameCount => FilterSequence.Length * SequenceList.Sum(s => s.Length);
}
