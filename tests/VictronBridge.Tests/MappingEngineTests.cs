using VictronBridge.Mapping;

namespace VictronBridge.Tests;

public sealed class MappingEngineTests
{
    private readonly MappingEngine _engine = new();

    [Fact]
    public void Map_MatchingTopics_ReturnsMappedValues()
    {
        var source = new Dictionary<string, object?>
        {
            ["battery/soc"] = 78.0,
            ["battery/voltage"] = 52.8,
            ["battery/current"] = -14.2,
        };

        var mappings = new Dictionary<string, string>
        {
            ["soc"] = "battery/soc",
            ["voltage"] = "battery/voltage",
            ["current"] = "battery/current",
        };

        var result = _engine.Map(source, mappings);

        Assert.Equal(78.0, result["soc"]);
        Assert.Equal(52.8, result["voltage"]);
        Assert.Equal(-14.2, result["current"]);
    }

    [Fact]
    public void Map_MissingSourceTopic_OmitsKeyFromResult()
    {
        var source = new Dictionary<string, object?>
        {
            ["battery/soc"] = 80.0,
        };

        var mappings = new Dictionary<string, string>
        {
            ["soc"] = "battery/soc",
            ["voltage"] = "battery/voltage", // not in source
        };

        var result = _engine.Map(source, mappings);

        Assert.True(result.ContainsKey("soc"));
        Assert.False(result.ContainsKey("voltage"));
    }

    [Fact]
    public void Map_EmptyMappings_ReturnsEmptyDictionary()
    {
        var source = new Dictionary<string, object?> { ["battery/soc"] = 80.0 };

        var result = _engine.Map(source, Enumerable.Empty<KeyValuePair<string, string>>());

        Assert.Empty(result);
    }

    [Fact]
    public void Map_EmptySource_ReturnsEmptyDictionary()
    {
        var mappings = new Dictionary<string, string> { ["soc"] = "battery/soc" };

        var result = _engine.Map(new Dictionary<string, object?>(), mappings);

        Assert.Empty(result);
    }

    [Fact]
    public void Map_NullSourceValue_MapsNullToDeviceKey()
    {
        var source = new Dictionary<string, object?> { ["battery/soc"] = null };
        var mappings = new Dictionary<string, string> { ["soc"] = "battery/soc" };

        var result = _engine.Map(source, mappings);

        Assert.True(result.ContainsKey("soc"));
        Assert.Null(result["soc"]);
    }

    [Fact]
    public void Map_MultipleMappingsToSameTopic_BothKeysPresent()
    {
        var source = new Dictionary<string, object?> { ["battery/soc"] = 90.0 };
        var mappings = new Dictionary<string, string>
        {
            ["soc"] = "battery/soc",
            ["stateOfCharge"] = "battery/soc",
        };

        var result = _engine.Map(source, mappings);

        Assert.Equal(90.0, result["soc"]);
        Assert.Equal(90.0, result["stateOfCharge"]);
    }

    [Fact]
    public void Map_IsCaseInsensitiveForSourceKeys()
    {
        var source = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["BATTERY/SOC"] = 75.0,
        };

        var mappings = new Dictionary<string, string>
        {
            ["soc"] = "battery/soc",
        };

        var result = _engine.Map(source, mappings);

        Assert.Equal(75.0, result["soc"]);
    }
}
