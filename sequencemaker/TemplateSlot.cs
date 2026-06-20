enum TemplateSlot
{
    Start,
    End,
    TargetPrep,
    SmartExposure,
    MeridianFlip,
    NauticalDawn,
    DsoShell,
    TargetAreaShell,
    ImagingContainerShell,
    FilterMap,
    // Future Phase 4 — optional failure alerts (disabled when alertTemplate is null)
    AlertTriggersRoot,
    AlertTriggersPrep,
    PrepFailureSentinel
}
