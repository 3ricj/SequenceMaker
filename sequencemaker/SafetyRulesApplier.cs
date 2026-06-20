using Newtonsoft.Json.Linq;

static class SafetyRulesApplier
{
    const int AbortOnError = 2;
    static readonly HashSet<string> PrepInstructionTypes = new(StringComparer.Ordinal)
    {
        "SwitchFilter",
        "CenterAndRotate",
        "RunAutofocus"
    };

    public static void Apply(JObject root, JObject? dawnTemplate = null)
    {
        foreach (var obj in NinaJsonHelper.WalkObjects(root))
        {
            if (obj.ContainsKey("IsExpanded"))
                obj["IsExpanded"] = false;
        }

        ApplyPrepAbort(root);
        ApplyImagingDawnCondition(root, dawnTemplate);
        StripMisplacedDawnConditions(root);
        EnforceFlatImageType(root);
    }

    static void ApplyPrepAbort(JObject root)
    {
        var prep = NinaJsonHelper.FindFirstByName(root, "Target preparation instructions", "Sequential");
        if (prep == null)
            return;

        foreach (var item in NinaJsonHelper.GetValues(prep, "Items").OfType<JObject>())
        {
            if (PrepInstructionTypes.Contains(NinaJsonHelper.ShortType(item)))
            {
                item["ErrorBehavior"] = AbortOnError;
                item["Attempts"] = 1;
            }
        }
    }

    static void ApplyImagingDawnCondition(JObject root, JObject? dawnTemplate)
    {
        var imaging = NinaJsonHelper.FindFirstByName(root, "Target imaging instructions", "Sequential");
        if (imaging == null)
            return;

        dawnTemplate ??= new JObject
        {
            ["$type"] = "NINA.Sequencer.Conditions.TimeCondition, NINA.Sequencer",
            ["Hours"] = 3,
            ["Minutes"] = 20,
            ["Seconds"] = 16,
            ["MinutesOffset"] = 0,
            ["SelectedProvider"] = new JObject
            {
                ["$type"] = "NINA.Sequencer.Utility.DateTimeProvider.NauticalDawnProvider, NINA.Sequencer"
            }
        };

        NinaJsonHelper.SetValues(
            imaging,
            "Conditions",
            new JArray { dawnTemplate.DeepClone() },
            NinaJsonHelper.ConditionCollectionType);
    }

    public static void ApplyImagingDawnFromTemplate(JObject root, JObject dawnTemplate) =>
        ApplyImagingDawnCondition(root, dawnTemplate);

    static void StripMisplacedDawnConditions(JObject root)
    {
        foreach (var container in NinaJsonHelper.WalkObjects(root))
        {
            var name = container["Name"]?.ToString();
            if (name is "Target imaging instructions")
                continue;
            if (!container.ContainsKey("Conditions"))
                continue;

            var values = NinaJsonHelper.GetValues(container, "Conditions");
            var filtered = new JArray(values
                .OfType<JObject>()
                .Where(c => !IsNauticalDawnCondition(c))
                .Cast<JToken>()
                .ToArray());

            NinaJsonHelper.SetValues(container, "Conditions", filtered, NinaJsonHelper.ConditionCollectionType);
        }
    }

    static bool IsNauticalDawnCondition(JObject condition)
    {
        if (NinaJsonHelper.ShortType(condition) != "TimeCondition")
            return false;
        return NinaJsonHelper.ShortType(condition["SelectedProvider"]) == "NauticalDawnProvider";
    }

    static void EnforceFlatImageType(JObject root)
    {
        foreach (var take in NinaJsonHelper.FindByType(root, "TakeExposure"))
            take["ImageType"] = "FLAT";
    }
}
