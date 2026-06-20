using Newtonsoft.Json.Linq;

class SequenceValidator
{
    private readonly List<string> _errors = [];

    public IReadOnlyList<string> Errors => _errors;

    public bool Validate(JObject root, SequenceConfig config)
    {
        _errors.Clear();
        ValidateDawnPlacement(root);
        ValidatePrepAbort(root);
        ValidateIsExpanded(root);
        ValidateMeridianFlip(root);
        ValidateSmartExposureCount(root, config);
        ValidateRefs(root);
        ValidateAlertTriggersIfEnabled(root, config);
        return _errors.Count == 0;
    }

    public void ValidateOrThrow(JObject root, SequenceConfig config)
    {
        if (!Validate(root, config))
            throw new InvalidOperationException("Sequence validation failed:\n- " + string.Join("\n- ", _errors));
    }

    void ValidateDawnPlacement(JObject root)
    {
        int dawnCount = 0;
        int dawnOnImaging = 0;
        int dawnElsewhere = 0;

        foreach (var container in NinaJsonHelper.WalkObjects(root).Where(o => o.ContainsKey("Conditions")))
        {
            foreach (var cond in NinaJsonHelper.GetValues(container, "Conditions").OfType<JObject>())
            {
                if (!IsNauticalDawn(cond))
                    continue;
                dawnCount++;
                if (container["Name"]?.ToString() == "Target imaging instructions")
                    dawnOnImaging++;
                else
                    dawnElsewhere++;
            }
        }

        if (dawnCount != 1)
            _errors.Add($"Expected exactly 1 Nautical Dawn TimeCondition, found {dawnCount}.");
        if (dawnOnImaging != 1)
            _errors.Add("Nautical Dawn TimeCondition must be on 'Target imaging instructions'.");
        if (dawnElsewhere > 0)
            _errors.Add("Nautical Dawn TimeCondition must not appear on Start/End or other containers.");
    }

    void ValidatePrepAbort(JObject root)
    {
        var prep = NinaJsonHelper.FindFirstByName(root, "Target preparation instructions", "Sequential");
        if (prep == null)
        {
            _errors.Add("Missing 'Target preparation instructions' container.");
            return;
        }

        foreach (var item in NinaJsonHelper.GetValues(prep, "Items").OfType<JObject>())
        {
            var t = NinaJsonHelper.ShortType(item);
            if (t is "SwitchFilter" or "CenterAndRotate" or "RunAutofocus")
            {
                if (item["ErrorBehavior"]?.Value<int>() != 2)
                    _errors.Add($"Prep instruction {t} must have ErrorBehavior=2 (Abort).");
            }
        }
    }

    void ValidateIsExpanded(JObject root)
    {
        foreach (var obj in NinaJsonHelper.WalkObjects(root))
        {
            if (obj.TryGetValue("IsExpanded", out var expanded) && expanded.Type == JTokenType.Boolean && expanded.Value<bool>())
                _errors.Add($"IsExpanded must be false (found true on {NinaJsonHelper.ShortType(obj)} '{obj["Name"]}').");
        }
    }

    void ValidateMeridianFlip(JObject root)
    {
        var triggers = NinaJsonHelper.GetValues(root, "Triggers");
        if (!triggers.OfType<JObject>().Any(t => NinaJsonHelper.ShortType(t).Contains("SmartMeridianFlip", StringComparison.Ordinal)))
            _errors.Add("Root must include SmartMeridianFlipTrigger.");
    }

    void ValidateSmartExposureCount(JObject root, SequenceConfig config)
    {
        int count = NinaJsonHelper.FindByType(root, "SmartExposure").Count();
        if (count != config.ExpectedFrameCount)
            _errors.Add($"Expected {config.ExpectedFrameCount} SmartExposure items, found {count}.");
    }

    void ValidateRefs(JObject root)
    {
        var ids = JsonReferenceRenumberer.CollectIds(root);
        foreach (var r in JsonReferenceRenumberer.CollectRefs(root))
        {
            if (!ids.Contains(r))
                _errors.Add($"Dangling $ref '{r}'.");
        }
    }

    void ValidateAlertTriggersIfEnabled(JObject root, SequenceConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.AlertTemplate))
            return;
        // Phase 4 hook — when alert templates exist, validate presence at graft points
        var rootTriggers = NinaJsonHelper.GetValues(root, "Triggers");
        if (!rootTriggers.Any())
            _errors.Add($"Alert template '{config.AlertTemplate}' enabled but no root triggers found.");
    }

    static bool IsNauticalDawn(JObject condition) =>
        NinaJsonHelper.ShortType(condition) == "TimeCondition" &&
        NinaJsonHelper.ShortType(condition["SelectedProvider"]) == "NauticalDawnProvider";
}
