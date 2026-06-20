using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class NinaAdvancedSequenceBuilder
{
    private readonly SequenceConfig _config;
    private readonly TemplateRegistry _registry;

    public NinaAdvancedSequenceBuilder(SequenceConfig config)
    {
        _config = config;
        _registry = new TemplateRegistry(config);
    }

    public JObject Build()
    {
        var root = new JObject
        {
            ["$type"] = "NINA.Sequencer.Container.SequenceRootContainer, NINA.Sequencer",
            ["Strategy"] = new JObject
            {
                ["$type"] = "NINA.Sequencer.Container.ExecutionStrategy.SequentialStrategy, NINA.Sequencer"
            },
            ["Name"] = _config.FileNameBase,
            ["Conditions"] = NinaJsonHelper.CreateObservableCollection(new JArray(), NinaJsonHelper.ConditionCollectionType),
            ["IsExpanded"] = false,
            ["ErrorBehavior"] = 0,
            ["Attempts"] = 1
        };

        var rootTriggers = new JArray { _registry.Clone(TemplateSlot.MeridianFlip) };
        GraftOptionalAlertTriggers(rootTriggers, TemplateSlot.AlertTriggersRoot);
        root["Triggers"] = NinaJsonHelper.CreateObservableCollection(rootTriggers, NinaJsonHelper.TriggerCollectionType);

        var items = new JArray
        {
            _registry.CloneStart(),
            BuildTargetArea(),
            _registry.CloneEnd()
        };
        root["Items"] = NinaJsonHelper.CreateObservableCollection(items);

        SafetyRulesApplier.Apply(root, _registry.Clone(TemplateSlot.NauticalDawn));

        var renumbered = JsonReferenceRenumberer.Renumber(root);
        new SequenceValidator().ValidateOrThrow(renumbered, _config);
        return renumbered;
    }

    public string BuildAndSerialize() =>
        JsonConvert.SerializeObject(Build(), Formatting.None);

    JObject BuildTargetArea()
    {
        var targetArea = _registry.Clone(TemplateSlot.TargetAreaShell);
        targetArea["Items"] = NinaJsonHelper.CreateObservableCollection(new JArray { BuildDeepSkyObject() });
        return targetArea;
    }

    JObject BuildDeepSkyObject()
    {
        if (!TargetCatalog.TryGet(_config.TargetObject, out var target))
            throw new InvalidOperationException($"Unknown target '{_config.TargetObject}'.");

        var dso = _registry.Clone(TemplateSlot.DsoShell);
        dso["Name"] = _config.TargetObject;

        var inputTarget = (JObject)dso["Target"]!;
        inputTarget["TargetName"] = _config.TargetObject;
        inputTarget["PositionAngle"] = target.PositionAngle;
        inputTarget["Expanded"] = false;

        var coords = (JObject)inputTarget["InputCoordinates"]!;
        coords["RAHours"] = int.Parse(target.RAHours);
        coords["RAMinutes"] = int.Parse(target.RAMinutes);
        coords["RASeconds"] = double.Parse(target.RASeconds);
        coords["NegativeDec"] = false;
        coords["DecDegrees"] = int.Parse(target.DecDegrees);
        coords["DecMinutes"] = int.Parse(target.DecMinutes);
        coords["DecSeconds"] = double.Parse(target.DecSeconds);

        var prep = BuildTargetPrep(target);
        var imaging = BuildImagingContainer();

        dso["Items"] = NinaJsonHelper.CreateObservableCollection(new JArray { prep, imaging });
        return dso;
    }

    JObject BuildTargetPrep(TargetInfo target)
    {
        var prep = _registry.Clone(TemplateSlot.TargetPrep);
        var items = NinaJsonHelper.GetValues(prep, "Items");

        foreach (var item in items.OfType<JObject>())
        {
            switch (NinaJsonHelper.ShortType(item))
            {
                case "SwitchFilter":
                    var firstFilter = _config.FilterSequence[0];
                    _registry.TryGetFilter(firstFilter, out var filterInfo);
                    item["Filter"] = filterInfo;
                    break;
                case "CenterAndRotate":
                    item["PositionAngle"] = target.PositionAngle;
                    item["Inherited"] = true;
                    var coords = (JObject)item["Coordinates"]!;
                    coords["RAHours"] = int.Parse(target.RAHours);
                    coords["RAMinutes"] = int.Parse(target.RAMinutes);
                    coords["RASeconds"] = double.Parse(target.RASeconds);
                    coords["NegativeDec"] = false;
                    coords["DecDegrees"] = int.Parse(target.DecDegrees);
                    coords["DecMinutes"] = int.Parse(target.DecMinutes);
                    coords["DecSeconds"] = double.Parse(target.DecSeconds);
                    break;
            }
        }

        GraftOptionalAlertTriggers(NinaJsonHelper.GetValues(prep, "Triggers"), TemplateSlot.AlertTriggersPrep);

        var prepItems = NinaJsonHelper.GetValues(prep, "Items");
        var sentinel = _registry.TryCloneOptional(TemplateSlot.PrepFailureSentinel);
        if (sentinel != null)
            prepItems.Add(sentinel);

        NinaJsonHelper.SetValues(prep, "Items", prepItems, NinaJsonHelper.ObservableCollectionType);
        return prep;
    }

    JObject BuildImagingContainer()
    {
        var imaging = _registry.Clone(TemplateSlot.ImagingContainerShell);
        var exposures = new JArray();

        foreach (var filter in _config.FilterSequence)
        {
            foreach (var sequence in _config.SequenceList)
            {
                foreach (var exposureTime in sequence)
                    exposures.Add(BuildSmartExposure(filter, exposureTime));
            }
        }

        NinaJsonHelper.SetValues(imaging, "Items", exposures, NinaJsonHelper.ObservableCollectionType);
        return imaging;
    }

    JObject BuildSmartExposure(string filterName, double exposureTime)
    {
        var smart = _registry.Clone(TemplateSlot.SmartExposure);
        _registry.TryGetFilter(filterName, out var filterInfo);

        foreach (var item in NinaJsonHelper.GetValues(smart, "Items").OfType<JObject>())
        {
            if (NinaJsonHelper.ShortType(item) == "SwitchFilter")
                item["Filter"] = filterInfo;
            else if (NinaJsonHelper.ShortType(item) == "TakeExposure")
                item["ExposureTime"] = exposureTime;
        }

        return smart;
    }

    void GraftOptionalAlertTriggers(JArray triggers, TemplateSlot slot)
    {
        if (string.IsNullOrWhiteSpace(_config.AlertTemplate))
            return;

        var fragment = _registry.TryCloneOptional(slot);
        if (fragment == null)
            return;

        if (fragment["$values"] is JArray values)
        {
            foreach (var t in values)
                triggers.Add(t);
        }
        else
        {
            triggers.Add(fragment);
        }
    }
}
