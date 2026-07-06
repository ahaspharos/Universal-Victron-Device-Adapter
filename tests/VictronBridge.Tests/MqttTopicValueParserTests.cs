using VictronBridge.Sources.Mqtt;

namespace VictronBridge.Tests;

public sealed class MqttTopicValueParserTests
{
    [Theory]
    [InlineData("42", 42L)]
    [InlineData("-10", -10L)]
    [InlineData("0", 0L)]
    public void Parse_IntegerPayload_ReturnsLong(string payload, long expected)
    {
        var result = MqttTopicValueParser.Parse(payload);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("3.14", 3.14)]
    [InlineData("-12.5", -12.5)]
    [InlineData("53.4", 53.4)]
    [InlineData("0.0", 0.0)]
    public void Parse_FloatPayload_ReturnsDouble(string payload, double expected)
    {
        var result = MqttTopicValueParser.Parse(payload);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("false", false)]
    [InlineData("False", false)]
    public void Parse_BoolPayload_ReturnsBool(string payload, bool expected)
    {
        var result = MqttTopicValueParser.Parse(payload);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("online")]
    [InlineData("charging")]
    [InlineData("hello world")]
    [InlineData("ON")]
    public void Parse_StringPayload_ReturnsString(string payload)
    {
        var result = MqttTopicValueParser.Parse(payload);
        Assert.Equal(payload, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Parse_EmptyOrNullPayload_ReturnsNull(string? payload)
    {
        var result = MqttTopicValueParser.Parse(payload!);
        Assert.Null(result);
    }

    [Fact]
    public void Parse_InvariantCultureFloat_ParsesCorrectly()
    {
        // Ensure dots are used as decimal separator, not commas
        var result = MqttTopicValueParser.Parse("52.8");
        Assert.Equal(52.8, result);
    }
}
