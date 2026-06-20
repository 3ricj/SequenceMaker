using Newtonsoft.Json.Linq;

static class JsonReferenceRenumberer
{
    public static JObject Renumber(JObject root)
    {
        int nextId = 1;
        AssignIds(root, ref nextId);
        WireParents(root);
        return root;
    }

    static void AssignIds(JToken token, ref int nextId)
    {
        switch (token)
        {
            case JObject obj:
                // $id MUST be the first property: Newtonsoft's PreserveReferencesHandling
                // only reads it as reference metadata when it precedes $type/$values/data.
                // Assigning via obj["$id"] = ... appends it for objects created without one,
                // which makes N.I.N.A. fail to deserialize the container.
                obj.Remove("$id");
                obj.AddFirst(new JProperty("$id", nextId.ToString()));
                nextId++;
                foreach (var prop in obj.Properties())
                {
                    if (prop.Name is "$ref" or "Parent")
                        continue;
                    AssignIds(prop.Value, ref nextId);
                }
                break;
            case JArray arr:
                foreach (var item in arr)
                    AssignIds(item, ref nextId);
                break;
        }
    }

    static void WireParents(JToken token)
    {
        if (token is not JObject obj)
            return;

        foreach (var prop in obj.Properties())
        {
            if (prop.Name is "Items" or "Triggers" or "Conditions" && prop.Value is JObject collection)
            {
                var values = collection["$values"] as JArray;
                var parentId = obj["$id"]?.ToString();
                if (values != null && parentId != null)
                {
                    foreach (var child in values.OfType<JObject>())
                    {
                        child["Parent"] = new JObject { ["$ref"] = parentId };
                        WireParents(child);
                    }
                }
            }
            else if (prop.Name != "Parent")
            {
                WireParents(prop.Value);
            }
        }
    }

    public static HashSet<string> CollectIds(JObject root) =>
        NinaJsonHelper.WalkObjects(root)
            .Select(o => o["$id"]?.ToString())
            .Where(id => !string.IsNullOrEmpty(id))
            .ToHashSet()!;

    public static IEnumerable<string> CollectRefs(JObject root)
    {
        foreach (var obj in NinaJsonHelper.WalkObjects(root))
        {
            if (obj["Parent"] is JObject parentRef)
            {
                var r = parentRef["$ref"]?.ToString();
                if (!string.IsNullOrEmpty(r))
                    yield return r;
            }
        }
    }
}
