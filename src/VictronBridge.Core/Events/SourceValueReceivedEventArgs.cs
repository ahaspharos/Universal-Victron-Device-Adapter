namespace VictronBridge.Core.Events;

public sealed class SourceValueReceivedEventArgs : EventArgs
{
    public string Key { get; }
    public object? Value { get; }
    public DateTimeOffset Timestamp { get; }

    public SourceValueReceivedEventArgs(string key, object? value, DateTimeOffset timestamp)
    {
        Key = key;
        Value = value;
        Timestamp = timestamp;
    }
}
