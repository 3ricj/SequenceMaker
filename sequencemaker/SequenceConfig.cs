class SequenceConfig
{
    /// <summary>Targets shot in order. Only the last target loops until Nautical Dawn.</summary>
    public List<TargetPlan> Targets { get; set; } = [];
    public string FileNameBase { get; set; } = "";
    public string? StartTemplateName { get; set; }
    public string? EndTemplateName { get; set; }
    /// <summary>Null = no failure alerts. Future: "ground-station-pushover", etc.</summary>
    public string? AlertTemplate { get; set; }
    public string NinaOutputFolder { get; set; } = "";
    public int ExpectedFrameCount => Targets.Sum(t => t.FrameCount);
}
