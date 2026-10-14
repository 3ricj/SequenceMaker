/// <summary>One filter shot with one exposure ramp, e.g. ("Red", SequenceRamps.Sequence3).</summary>
record ExposureBlock(string Filter, double[] Ramp);

/// <summary>One target (panel) and its exposure blocks, shot in the order listed.</summary>
record TargetPlan(string TargetName, List<ExposureBlock> Blocks)
{
    /// <summary>Old-style plan: every ramp shot on every filter (filter-major order).</summary>
    public static TargetPlan EveryFilter(string targetName, string[] filters, List<double[]> ramps) =>
        new(targetName, filters.SelectMany(f => ramps.Select(r => new ExposureBlock(f, r))).ToList());

    public int FrameCount => Blocks.Sum(b => b.Ramp.Length);

    public double ExposureSeconds => Blocks.Sum(b => b.Ramp.Sum());
}
