using VictronBridge.Core.Abstractions;

namespace VictronBridge.Models;

public abstract class DeviceModelBase : IDeviceModel
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.OrdinalIgnoreCase);

    public abstract string ServiceName { get; }
    public abstract string DeviceType { get; }

    protected void SetValue(string path, object? value) => _values[path] = value;

    protected object? GetValue(string path) =>
        _values.TryGetValue(path, out var v) ? v : null;

    public IReadOnlyDictionary<string, object?> GetDbusValues() =>
        _values.AsReadOnly();

    public abstract void ApplyValues(IReadOnlyDictionary<string, object?> values);
}
