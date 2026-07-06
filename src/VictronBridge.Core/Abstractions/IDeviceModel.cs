namespace VictronBridge.Core.Abstractions;

public interface IDeviceModel
{
    string ServiceName { get; }

    string DeviceType { get; }

    IReadOnlyDictionary<string, object?> GetDbusValues();

    void ApplyValues(IReadOnlyDictionary<string, object?> values);
}
