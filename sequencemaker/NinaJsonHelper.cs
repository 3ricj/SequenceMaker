using Newtonsoft.Json.Linq;

static class NinaJsonHelper
{
    public const string ObservableCollectionType =
        "System.Collections.ObjectModel.ObservableCollection`1[[NINA.Sequencer.SequenceItem.ISequenceItem, NINA.Sequencer]], System.ObjectModel";

    public const string ConditionCollectionType =
        "System.Collections.ObjectModel.ObservableCollection`1[[NINA.Sequencer.Conditions.ISequenceCondition, NINA.Sequencer]], System.ObjectModel";

    public const string TriggerCollectionType =
        "System.Collections.ObjectModel.ObservableCollection`1[[NINA.Sequencer.Trigger.ISequenceTrigger, NINA.Sequencer]], System.ObjectModel";

    public static string ShortType(JToken? token) =>
        ShortTypeName(token?["$type"]?.ToString());

    public static string ShortTypeName(string? fullType) =>
        string.IsNullOrEmpty(fullType)
            ? ""
            : fullType.Split(',')[0].Split('.')[^1];

    public static JObject DeepClone(JObject source) =>
        (JObject)source.DeepClone();

    public static JObject CreateObservableCollection(JArray values, string collectionType = ObservableCollectionType) =>
        new()
        {
            ["$type"] = collectionType,
            ["$values"] = values
        };

    public static JArray GetValues(JObject container, string propertyName) =>
        (JArray)(container[propertyName]?["$values"] ?? new JArray());

    public static void SetValues(JObject container, string propertyName, JArray values, string collectionType)
    {
        container[propertyName] = CreateObservableCollection(values, collectionType);
    }

    public static IEnumerable<JObject> WalkObjects(JToken root)
    {
        if (root is JObject obj)
        {
            yield return obj;
            foreach (var prop in obj.Properties())
            {
                foreach (var child in WalkObjects(prop.Value))
                    yield return child;
            }
        }
        else if (root is JArray arr)
        {
            foreach (var item in arr)
            {
                foreach (var child in WalkObjects(item))
                    yield return child;
            }
        }
    }

    public static JObject? FindFirstByName(JToken root, string name, string? typeContains = null)
    {
        foreach (var obj in WalkObjects(root))
        {
            if (obj["Name"]?.ToString() != name)
                continue;
            if (typeContains != null && !ShortType(obj).Contains(typeContains, StringComparison.Ordinal))
                continue;
            return obj;
        }
        return null;
    }

    public static IEnumerable<JObject> FindByType(JToken root, string shortTypeName)
    {
        foreach (var obj in WalkObjects(root))
        {
            if (ShortType(obj) == shortTypeName)
                yield return obj;
        }
    }
}
