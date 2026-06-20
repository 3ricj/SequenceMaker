using Newtonsoft.Json.Linq;
using Xunit;

public class JsonReferenceRenumbererTests
{
    [Fact]
    public void Renumber_AssignsSequentialIdsAndValidParentRefs()
    {
        var root = new JObject
        {
            ["$type"] = "NINA.Sequencer.Container.SequenceRootContainer, NINA.Sequencer",
            ["Items"] = NinaJsonHelper.CreateObservableCollection(new JArray
            {
                new JObject
                {
                    ["$type"] = "NINA.Sequencer.Container.SequentialContainer, NINA.Sequencer",
                    ["Name"] = "child",
                    ["Items"] = NinaJsonHelper.CreateObservableCollection(new JArray
                    {
                        new JObject
                        {
                            ["$type"] = "NINA.Sequencer.SequenceItem.Utility.Annotation, NINA.Sequencer",
                            ["Text"] = "test"
                        }
                    })
                }
            })
        };

        var result = JsonReferenceRenumberer.Renumber(root);
        var ids = JsonReferenceRenumberer.CollectIds(result);
        var refs = JsonReferenceRenumberer.CollectRefs(result).ToList();

        Assert.True(ids.Count >= 4);
        Assert.Equal(ids.Count, ids.Select(id => id).Distinct().Count());
        Assert.All(refs, r => Assert.Contains(r, ids));
    }

    [Fact]
    public void Renumber_PlacesIdAsFirstProperty_OnObjectsCreatedWithoutOne()
    {
        // Collections built via CreateObservableCollection start with $type, not $id.
        // Newtonsoft's PreserveReferencesHandling requires $id to come first, otherwise
        // deserialization throws "Unexpected token when deserializing object: String".
        var root = new JObject
        {
            ["$type"] = "NINA.Sequencer.Container.SequenceRootContainer, NINA.Sequencer",
            ["Conditions"] = NinaJsonHelper.CreateObservableCollection(
                new JArray(), NinaJsonHelper.ConditionCollectionType),
            ["Items"] = NinaJsonHelper.CreateObservableCollection(new JArray())
        };

        var result = JsonReferenceRenumberer.Renumber(root);

        foreach (var obj in NinaJsonHelper.WalkObjects(result))
        {
            if (obj["$id"] == null)
                continue;
            Assert.Equal("$id", obj.Properties().First().Name);
        }
    }

    [Fact]
    public void Builder_ProducesValidSequence_WithSafetyRules()
    {
        var config = new SequenceConfig
        {
            TargetObject = "gammacygni",
            FilterSequence = ["Red"],
            SequenceList = [[1.0, 2.0, 3.0]],
            FileNameBase = "test-sequence",
            NinaOutputFolder = Path.GetTempPath()
        };

        var root = new NinaAdvancedSequenceBuilder(config).Build();
        var validator = new SequenceValidator();
        Assert.True(validator.Validate(root, config), string.Join("; ", validator.Errors));

        var prep = NinaJsonHelper.FindFirstByName(root, "Target preparation instructions", "Sequential");
        Assert.NotNull(prep);
        foreach (var item in NinaJsonHelper.GetValues(prep, "Items").OfType<JObject>())
        {
            var t = NinaJsonHelper.ShortType(item);
            if (t is "SwitchFilter" or "CenterAndRotate" or "RunAutofocus")
                Assert.Equal(2, item["ErrorBehavior"]!.Value<int>());
        }

        var imaging = NinaJsonHelper.FindFirstByName(root, "Target imaging instructions", "Sequential");
        Assert.NotNull(imaging);
        var dawn = NinaJsonHelper.GetValues(imaging, "Conditions").OfType<JObject>().Single();
        Assert.Equal("NauticalDawnProvider", NinaJsonHelper.ShortType(dawn["SelectedProvider"]));

        Assert.Equal(3, NinaJsonHelper.FindByType(root, "SmartExposure").Count());
    }
}
