namespace VictronBridge.Sources.Mqtt;

internal static class MqttTopicValueParser
{
    internal static object? Parse(string payload)
    {
        if (string.IsNullOrEmpty(payload))
            return null;

        if (long.TryParse(payload, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var longValue))
            return longValue;

        if (double.TryParse(payload, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
            return doubleValue;

        if (bool.TryParse(payload, out var boolValue))
            return boolValue;

        return payload;
    }
}
