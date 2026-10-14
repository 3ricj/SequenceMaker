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
        var dawnContainer = ApplyImagingDawnCondition(root, dawnTemplate);
        StripMisplacedDawnConditions(root, dawnContainer);
        EnforceFlatImageType(root);
    }

    static void ApplyPrepAbort(JObject root)
    {
        foreach (var prep in NinaJsonHelper.FindAllByName(root, "Target preparation instructions", "Sequential"))
        {
            foreach (var item in NinaJsonHelper.GetValues(prep, "Items").OfType<JObject>())
            {
                if (PrepInstructionTypes.Contains(NinaJsonHelper.ShortType(item)))
                {
                    item["ErrorBehavior"] = AbortOnError;
                    item["Attempts"] = 1;
                }
            }
        }
    }

    /// <summary>
    /// The dawn loop goes on the LAST target's imaging container only; on an earlier target it
    /// would repeat that target all night and the later targets would never run.
    /// </summary>
    static JObject? ApplyImagingDawnCondition(JObject root, JObject? dawnTemplate)
    {
        var imaging = NinaJsonHelper.FindAllByName(root, "Target imaging instructions", "Sequential").LastOrDefault();
        if (imaging == null)
            return null;

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
        return imaging;
    }

    public static void ApplyImagingDawnFromTemplate(JObject root, JObject dawnTemplate) =>
        ApplyImagingDawnCondition(root, dawnTemplate);

    static void StripMisplacedDawnConditions(JObject root, JObject? dawnContainer)
    {
        foreach (var container in NinaJsonHelper.WalkObjects(root).ToList())
        {
            if (ReferenceEquals(container, dawnContainer))
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
