using Newtonsoft.Json.Linq;

class TemplateRegistry
{
    private static readonly Dictionary<TemplateSlot, string> BundledFileNames = new()
    {
        [TemplateSlot.Start] = "start-basic.json",
        [TemplateSlot.End] = "end-basic.json",
        [TemplateSlot.TargetPrep] = "target-prep.json",
        [TemplateSlot.SmartExposure] = "smart-exposure.json",
        [TemplateSlot.MeridianFlip] = "meridian-flip.json",
        [TemplateSlot.NauticalDawn] = "nautical-dawn.json",
        [TemplateSlot.DsoShell] = "dso-shell.json",
        [TemplateSlot.TargetAreaShell] = "target-area-shell.json",
        [TemplateSlot.ImagingContainerShell] = "imaging-container-shell.json",
        [TemplateSlot.FilterMap] = "filter-map.json",
        [TemplateSlot.AlertTriggersRoot] = "alert-triggers-root.json",
        [TemplateSlot.AlertTriggersPrep] = "alert-triggers-prep.json",
        [TemplateSlot.PrepFailureSentinel] = "prep-failure-sentinel.json",
    };

    private readonly string _bundledFolder;
    private readonly SequenceConfig _config;
    private readonly Dictionary<string, JObject> _filterMap = new(StringComparer.OrdinalIgnoreCase);

    public TemplateRegistry(SequenceConfig config)
    {
        _config = config;
        _bundledFolder = Path.Combine(AppContext.BaseDirectory, "templates");
        LoadFilterMap();
    }

    public JObject Clone(TemplateSlot slot)
    {
        var source = LoadBundled(slot);
        return NinaJsonHelper.DeepClone(source);
    }

    public JObject CloneStart() =>
        _config.StartTemplateName != null
            ? NinaJsonHelper.DeepClone(LoadExternalAreaTemplate(_config.StartTemplateName, "StartAreaContainer"))
            : Clone(TemplateSlot.Start);

    public JObject CloneEnd() =>
        _config.EndTemplateName != null
            ? NinaJsonHelper.DeepClone(LoadExternalAreaTemplate(_config.EndTemplateName, "EndAreaContainer"))
            : Clone(TemplateSlot.End);

    public JObject? TryCloneOptional(TemplateSlot slot)
    {
        if (!BundledFileNames.TryGetValue(slot, out var fileName))
            return null;
        var path = Path.Combine(_bundledFolder, fileName);
        if (!File.Exists(path))
            return null;
        return Clone(slot);
    }

    public bool TryGetFilter(string filterName, out JObject filterInfo)
    {
        if (_filterMap.TryGetValue(filterName, out var info))
        {
            filterInfo = NinaJsonHelper.DeepClone(info);
            return true;
        }
        filterInfo = new JObject
        {
            ["$type"] = "NINA.Core.Model.Equipment.FilterInfo, NINA.Core",
            ["_name"] = filterName,
            ["_focusOffset"] = 0,
            ["_position"] = 0,
            ["_autoFocusExposureTime"] = -1.0,
            ["_autoFocusFilter"] = false,
            ["_autoFocusGain"] = -1,
            ["_autoFocusOffset"] = -1,
            ["_autoFocusBinning"] = new JObject
            {
                ["$type"] = "NINA.Core.Model.Equipment.BinningMode, NINA.Core",
                ["X"] = 1,
                ["Y"] = 1
            }
        };
        return false;
    }

    public static IEnumerable<string> DiscoverNinaTemplateFolders()
    {
        var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        yield return Path.Combine(docs, "N.I.N.A", "Templates");
        var oneDrive = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive", "Documents", "N.I.N.A", "Templates");
        if (Directory.Exists(oneDrive))
            yield return oneDrive;
    }

    public static IReadOnlyList<string> ListExternalTemplateNames()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var folder in DiscoverNinaTemplateFolders())
        {
            if (!Directory.Exists(folder))
                continue;
            foreach (var file in Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories))
            {
                try
                {
                    var json = LoadJsonFile(file);
                    var name = json["Name"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(name))
                        names.Add(name);
                    else
                        names.Add(Path.GetFileNameWithoutExtension(file));
                }
                catch
                {
                    names.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }
        return names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
    }

    JObject LoadBundled(TemplateSlot slot)
    {
        if (!BundledFileNames.TryGetValue(slot, out var fileName))
            throw new InvalidOperationException($"No bundled template for slot {slot}");
        var path = Path.Combine(_bundledFolder, fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Bundled template not found: {path}", path);
        return LoadJsonFile(path);
    }

    JObject LoadExternalAreaTemplate(string templateName, string expectedAreaType)
    {
        foreach (var folder in DiscoverNinaTemplateFolders())
        {
            if (!Directory.Exists(folder))
                continue;
            foreach (var file in Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories))
            {
                var json = LoadJsonFile(file);
                var name = json["Name"]?.ToString() ?? Path.GetFileNameWithoutExtension(file);
                if (!name.Equals(templateName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (NinaJsonHelper.ShortType(json).Contains(expectedAreaType, StringComparison.Ordinal))
                    return json;

                var nested = NinaJsonHelper.FindFirstByName(json, expectedAreaType switch
                {
                    "StartAreaContainer" => "Start",
                    "EndAreaContainer" => "End",
                    _ => templateName
                }, expectedAreaType.Replace("Container", ""));
                if (nested != null)
                    return nested;
            }
        }
        throw new FileNotFoundException($"External template '{templateName}' not found in N.I.N.A Templates folders.");
    }

    void LoadFilterMap()
    {
        try
        {
            var map = LoadBundled(TemplateSlot.FilterMap);
            foreach (var prop in map.Properties())
            {
                if (prop.Value is JObject filter)
                    _filterMap[prop.Name] = filter;
            }
        }
        catch (FileNotFoundException)
        {
            // filter-map optional at runtime if missing
        }
    }

    static JObject LoadJsonFile(string path)
    {
        using var reader = new StreamReader(path);
        using var jsonReader = new Newtonsoft.Json.JsonTextReader(reader);
        return JObject.Load(jsonReader);
    }
}
